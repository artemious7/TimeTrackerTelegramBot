variable "resource_group" {
  description = "Existing resource group."
  type        = string
  default     = "TimeTrackerTelegramBot"
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
