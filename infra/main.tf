# Resource group
resource "azurerm_resource_group" "default" {
  name     = "timeTrackerBot-${module.naming.resource_group.name_unique}"
  location = var.location
}

# Function app
module "function_app" {
  source                              = "Azure/avm-res-web-site/azurerm"
  version                             = "0.9.1"
  location                            = var.location
  resource_group_name                 = azurerm_resource_group.default.name
  name                                = "${module.naming.function_app.name_unique}-default"
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
}
