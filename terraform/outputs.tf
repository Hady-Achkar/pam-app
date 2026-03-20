output "frontend_url" {
  description = "Public URL of the frontend app"
  value       = "https://${azurerm_container_app.frontend.ingress[0].fqdn}"
}

output "backend_url" {
  description = "Public URL of the backend API"
  value       = "https://${azurerm_container_app.backend.ingress[0].fqdn}"
}

output "acr_login_server" {
  description = "ACR login server for CI/CD"
  value       = azurerm_container_registry.acr.login_server
}

output "storage_account_name" {
  description = "Storage account name"
  value       = azurerm_storage_account.storage.name
}
