locals {
  tags = {
    Application = "TimeTrackerTelegramBot"
    Creator     = var.creator_tag
  }
}

# Resource group
data "azurerm_resource_group" "default" {
  name = var.resource_group
}

# Function app
module "function_app" {
  source                              = "git::https://github.com/Azure/terraform-azurerm-avm-res-web-site.git?ref=b8d63f47fc2cbfa21aa362d39a08ea38a9b31d36" # commit hash of version 0.9.1
  location                            = data.azurerm_resource_group.default.location
  resource_group_name                 = data.azurerm_resource_group.default.name
  name                                = module.naming.function_app.name_unique
  kind                                = "functionapp"
  os_type                             = "Windows"
  enable_telemetry                    = true
  create_service_plan                 = true
  function_app_create_storage_account = true

  new_service_plan = {
    sku_name = "Y1"
  }

  function_app_storage_account = {
    name = module.naming.storage_account.name_unique
  }

  app_settings = {
    dataContainerName                      = "time-tracker-data"
    TelegramBotApiKey                      = var.TelegramBotApiKey
    FUNCTIONS_INPROC_NET8_ENABLED          = "1"
    FUNCTIONS_WORKER_RUNTIME               = "dotnet-isolated"
    WEBSITE_RUN_FROM_PACKAGE               = "1"
    WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED = "1"
  }

  tags = local.tags
}
