# TimeTracker Telegram Bot

This bot is designed to help you efficiently track and report your working time through a convenient and intuitive interface.

Try it live at [@turbo_time_tracker_bot](https://t.me/turbo_time_tracker_bot)

## Motivation

I built this project to solve a common problem I faced in my personal and professional life - tracking the amount of time spent on work throughout the month. I found that traditional methods like using mobile apps were too complicated and lacked the convenience and real-time accessibility I needed. As an avid user of Telegram, I decided to create this bot to integrate seamlessly with my daily workflow. Now, tracking time has never been easier or more convenient!

## Quick Start

To get started with the TimeTracker Telegram Bot, follow these simple steps.

### Registering Your Telegram Bot

To register your Telegram bot using BotFather, follow these steps:

1. **Open Telegram and Start a Chat with BotFather**:
   - Search for `@BotFather` in the Telegram search bar or click [this link](https://t.me/BotFather) to start a chat with BotFather.

2. **Create a New Bot**:
   - Type `/start` to begin the conversation with BotFather if you haven't already.
   - Type `/newbot` to create a new bot.

3. **Choose a Name for Your Bot**:
   - BotFather will ask you for a name for your bot. This is the name that will be displayed to users.
   - For example: `My Time Tracker Bot`

4. **Choose a Username for Your Bot**:
   - Next, you will be prompted to choose a username for your bot. The username must end in `bot` (e.g., `time_tracker_6709_bot`).

5. **Receive Your Bot Token**:
   - After choosing the username, BotFather will provide you with an API token. This token is essential for interacting with the Telegram Bot API and must be kept confidential.
   - Copy the token; you will need it to configure your bot in the next steps.

### Running locally

All the examples below use the `bash` shell.

1. Clone this repository to your local machine:
    ```bash
    git clone https://github.com/artemious7/TimeTrackerTelegramBot.git
    ```
2. Navigate into the project `src` directory:
    ```bash
    cd TimeTrackerTelegramBot/src
    ```
3. Register your Telegram bot via BotFather to get the API key and set Telegram Bot API key via the secrets file for the project:

    ```bash
    dotnet user-secrets init
    TelegramApiKey=<Your_Api_Key>
    dotnet user-secrets set TelegramBotApiKey $TelegramApiKey
    ```
  
1. Install Azurite to use local storage account. If you want to use an Azure Storage account, add the connection string to the secrets file:

    ```bash
    dotnet user-secrets set AzureWebJobsStorage <Your_Connection_String>
    ```

4. Build the project:
    ```bash
    dotnet build
    ```
5. Install Azure Functions Core Tools and run the Function app locally:
    ```bash
    func start
    ```
6. Use ngrok or other similar tool to direct Telegram to your app running locally.
    ```bash
    ngrok http 7287
    ```
7. Copy the forwarding URL provided by ngrok and configure your Telegram bot webhook with the following command:
    ```bash
    curl -F "url=<Your_Ngrok_URL>/api/ProcessUpdate" https://api.telegram.org/bot$TelegramApiKey/setWebhook
    ```
8. Send `/start` message to the Telegram bot.

### Deployment to Azure (optional)
If you are ready to deploy your bot to Azure, follow these steps:

1. Install the Azure CLI: [Azure CLI installation guide](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli).
2. Log in to your Azure account.
    ```bash
    az login
    ```
3. Create a new Function App in your resource group.
    ```bash
    TelegramApiKey=<Your_Api_Key>
    rand=$RANDOM

    az group create \
      --name TimeTrackerTelegramBot-rg \
      --location northeurope

    az storage account create \
      --resource-group TimeTrackerTelegramBot-rg \
      --name timetrackersa$rand \
      --location northeurope

    az functionapp create \
      --resource-group TimeTrackerTelegramBot-rg \
      --consumption-plan-location northeurope \
      --runtime dotnet \
      --functions-version 4 \
      --name TimeTrackerTelegramBot-$rand \
      --storage-account timetrackersa$rand
    
    # set app settings. Without these settings, Function app will be in error
    az functionapp config appsettings set \
      --resource-group TimeTrackerTelegramBot-rg \
      --name TimeTrackerTelegramBot-$rand \
      --settings "TelegramBotApiKey=$TelegramApiKey" "dataContainerName=time-tracker-data-production"
    ```
4. Install [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local#install-the-azure-functions-core-tools).  
1. Deploy your code to Azure.
    ```bash
    func azure functionapp publish TimeTrackerTelegramBot-$rand
    ```
1. Get the `ProcessUpdate` function key to authorize the requests:
   ```bash
   code=$(az functionapp function keys list \
     --resource-group TimeTrackerTelegramBot-rg \
     --name TimeTrackerTelegramBot-$rand \
     --function-name ProcessUpdate \
     --query default \
     --output tsv | tr -d '\r')
   ```
5. Update the webhook setting on Telegram to point to your Azure-hosted endpoint.
    ```bash
    curl -F "url=https://TimeTrackerTelegramBot-$rand.azurewebsites.net/api/ProcessUpdate?code=$code" https://api.telegram.org/bot$TelegramApiKey/setWebhook
    ```


## Usage

Once the bot is up and running, you can interact with it via Telegram. Here are the commands and instructions to help you get started:
- `/start`: Welcome message and instructions on how to use the bot.
- `/help`: Detailed instructions on how to log your time.
- `/reset`: Reset the total recorded time.
- `/showTotal`: Display the total recorded time.
- `/undo`: Undo the last recorded time entry.
- **Logging Time**: You can log time by simply sending messages in the format `1:35`, `15:45 - 16:20`, or `-0:20` to subtract time.

## Contributing

Here are some ways you can contribute:
- **Report Issues**: Use the GitHub issue tracker to report bugs or suggest improvements.
- **Submit Pull Requests**: Fork the repository, implement your changes, and submit a pull request for review.
- **Share Ideas**: Feel free to share your ideas for new features and improvements.

### How to Contribute

1. Fork the repository.
2. Create a new branch for your feature or bugfix:
    ```bash
    git checkout -b feature-name
    ``` 
3. Commit your changes:
    ```bash
    git commit -m "Description of your feature or fix"
    ```
4. Push to the branch:
    ```bash
    git push origin feature-name
    ```
5. Submit a pull request through GitHub.

Thank you for contributing to the TimeTracker Telegram Bot!

## License

This project is licensed under the MIT License. See the LICENSE file for more details.