# Resource group
resource "azurerm_resource_group" "group" {
    name = "timeTrackerBot-${module.naming.resource_group.name_unique}" 
    location = var.location
}

# Function app
module "function_app" {
  source  = "Azure/avm-res-web-site/azurerm"
  version = "0.9.1"
  location = var.location
  resource_group_name = azurerm_resource_group.group.name
  name = "${module.naming.function_app.name_unique}-default"
  kind = "functionapp"
  os_type = "Windows"

  enable_telemetry = true
  app_settings = {
      dataContainerName = "time-tracker-data"
      FUNCTIONS_INPROC_NET8_ENABLED = "1"
      FUNCTIONS_WORKER_RUNTIME = "dotnet-isolated"
      TelegramBotApiKey = var.TelegramBotApiKey
      WEBSITE_RUN_FROM_PACKAGE = "1"
      WEBSITE_USE_PLACEHOLDER_DOTNETISOLATED = "1"
  }

  create_service_plan = true
  new_service_plan = {
    sku_name = "Y1"
  }

  function_app_create_storage_account = true
  function_app_storage_account = {
    name = module.naming.storage_account.name_unique
  }
}
