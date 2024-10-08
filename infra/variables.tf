variable "location" {
  type        = string
  default     = "uksouth"
  description = "Location (region) of the resources"
}

variable "TelegramBotApiKey" {
  type        = string
  sensitive   = true
  description = "Telegram Bot API key"
}
