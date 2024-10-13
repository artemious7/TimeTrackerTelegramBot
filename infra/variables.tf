variable "location" {
  description = "Location of the resources."
  type        = string
  default     = "uksouth"
}

variable "TelegramBotApiKey" {
  description = "Telegram Bot API key."
  type        = string
  sensitive   = true
}

variable "creator_tag" {
  description = "'Creator' tag that will be set on created resources."
  type        = string
  default     = null
}
