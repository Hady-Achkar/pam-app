variable "location" {
  description = "Azure region"
  type        = string
  default     = "eastus"
}

variable "project_name" {
  description = "Project name used in resource naming"
  type        = string
  default     = "pam"
}

variable "acr_name" {
  description = "Globally unique Azure Container Registry name (alphanumeric only)"
  type        = string
}

variable "storage_account_name" {
  description = "Globally unique Storage Account name (alphanumeric only, 3-24 chars)"
  type        = string
}

variable "backend_image_tag" {
  description = "Docker image tag for the backend container"
  type        = string
  default     = "latest"
}

variable "frontend_image_tag" {
  description = "Docker image tag for the frontend container"
  type        = string
  default     = "latest"
}
