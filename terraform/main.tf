terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# resource group

resource "azurerm_resource_group" "rg" {
  name     = "${var.project_name}-rg"
  location = var.location
}

# log analytics, required for container apps

resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.project_name}-law"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

# container app environment, shared by both apps

resource "azurerm_container_app_environment" "cae" {
  name                       = "${var.project_name}-cae"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}

# acr - azure container registry

resource "azurerm_container_registry" "acr" {
  name                = var.acr_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "Basic"
  admin_enabled       = false
}

# storage account

resource "azurerm_storage_account" "storage" {
  name                     = var.storage_account_name
  location                 = azurerm_resource_group.rg.location
  resource_group_name      = azurerm_resource_group.rg.name
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

# storage container for photos
resource "azurerm_storage_container" "photos" {
  name                  = "photos"
  storage_account_id    = azurerm_storage_account.storage.id
  container_access_type = "blob"
}


# created before container apps so AcrPull role is ready at deploy time
resource "azurerm_user_assigned_identity" "backend_identity" {
  name                = "${var.project_name}-backend-id"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_user_assigned_identity" "frontend_identity" {
  name                = "${var.project_name}-frontend-id"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}

resource "azurerm_role_assignment" "backend_acr_pull" {
  scope                = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.backend_identity.principal_id
}

resource "azurerm_role_assignment" "frontend_acr_pull" {
  scope                = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.frontend_identity.principal_id
}

# backend container app — can scale to 0 when not in use
resource "azurerm_container_app" "backend" {
  name                         = "${var.project_name}-ca-backend"
  resource_group_name          = azurerm_resource_group.rg.name
  container_app_environment_id = azurerm_container_app_environment.cae.id
  revision_mode                = "Single"

  depends_on = [azurerm_role_assignment.backend_acr_pull]

  identity {
    type         = "SystemAssigned, UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.backend_identity.id]
  }

  registry {
    server   = azurerm_container_registry.acr.login_server
    identity = azurerm_user_assigned_identity.backend_identity.id
  }

  ingress {
    external_enabled = true
    target_port      = 8080
    transport        = "auto"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = 0
    max_replicas = 1

    container {
      name   = "backend"
      image  = "${azurerm_container_registry.acr.login_server}/pam-backend:${var.backend_image_tag}"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }

      env {
        name  = "Storage__Provider"
        value = "AzureBlob"
      }

      env {
        name  = "Storage__AzureBlob__ServiceUri"
        value = "https://${azurerm_storage_account.storage.name}.blob.core.windows.net"
      }

      env {
        name  = "Cors__AllowedOrigins__0"
        value = "https://${azurerm_container_app.frontend.ingress[0].fqdn}"
      }
    }
  }
}

# frontend container app
resource "azurerm_container_app" "frontend" {
  name                         = "${var.project_name}-ca-frontend"
  resource_group_name          = azurerm_resource_group.rg.name
  container_app_environment_id = azurerm_container_app_environment.cae.id
  revision_mode                = "Single"

  depends_on = [azurerm_role_assignment.frontend_acr_pull]

  identity {
    type         = "UserAssigned"
    identity_ids = [azurerm_user_assigned_identity.frontend_identity.id]
  }

  registry {
    server   = azurerm_container_registry.acr.login_server
    identity = azurerm_user_assigned_identity.frontend_identity.id
  }

  ingress {
    external_enabled = true
    target_port      = 80
    transport        = "auto"

    traffic_weight {
      latest_revision = true
      percentage      = 100
    }
  }

  template {
    min_replicas = 0
    max_replicas = 1

    container {
      name   = "frontend"
      image  = "${azurerm_container_registry.acr.login_server}/pam-frontend:${var.frontend_image_tag}"
      cpu    = 0.25
      memory = "0.5Gi"
    }
  }
}

# custom role — only read + write blobs, no delete (principle of least privilege)
resource "azurerm_role_definition" "blob_photo_uploader" {
  name        = "Blob Photo Uploader"
  scope       = azurerm_resource_group.rg.id
  description = "Can read and write blobs in the photos container. Cannot delete."

  permissions {
    actions = [
      "Microsoft.Storage/storageAccounts/blobServices/containers/read"
    ]
    data_actions = [
      "Microsoft.Storage/storageAccounts/blobServices/containers/blobs/read",
      "Microsoft.Storage/storageAccounts/blobServices/containers/blobs/write"
    ]
  }

  assignable_scopes = [azurerm_resource_group.rg.id]
}

# assign custom role to backend system identity, scoped to the photos container only
resource "azurerm_role_assignment" "backend_blob_access" {
  scope              = azurerm_storage_container.photos.id
  role_definition_id = azurerm_role_definition.blob_photo_uploader.role_definition_resource_id
  principal_id       = azurerm_container_app.backend.identity[0].principal_id
}
