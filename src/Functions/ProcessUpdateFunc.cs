using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using TimeTrackerBot.Data;
using TimeTrackerBot.TimeTracker;
using TimeTrackerBot.Services;
using System.Net;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TimeTrackerBot.Functions;
public class ProcessUpdateFunc(ILogger<ProcessUpdateFunc> logger, IConfiguration configuration, ResponderFactory responderFactory)
{
    private readonly TelegramBotClient? botClient = configuration[SettingsKeys.TelegramBotApiKey] is { } key ? new(key) : null;
    private const string UserDataBlobNamePattern = "%dataContainerName%/chat/{message.chat.id}.json";

    [Function(nameof(ProcessUpdate))]
    [BlobOutput(UserDataBlobNamePattern)]
    public async Task<UserData?> ProcessUpdate(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request,
        [FromBody] Update update,
        [BlobInput(UserDataBlobNamePattern)] BlobClient userDataBlobClient)
    {
        using var loggerScope = logger.BeginScope("ChatId: {chatId}, MessageId: {messageId}",
            update?.Message?.Chat?.Id, update?.Message?.Id);
        // message content is not logged in production unless trace log level is enabled
        logger.LogTrace("Received update: {update}", JsonSerializer.Serialize(update));

        HttpResponseData response = request.CreateResponse(HttpStatusCode.OK);
        if (update is not { Message: { Text: { } message, Chat.Id: { } chatId } })
        {
            logger.LogInformation("No text message in the Update.");
            return null;
        }

        UserData? userData = await GetUserDataIfExists();

        logger.LogTrace($"{nameof(UserData)} before processing the Update: {{data}}", JsonSerializer.Serialize(userData));

        var responder = responderFactory(message, userData, SendMessage);
        userData = await responder.Process();

        logger.LogInformation("Update processed");
        logger.LogTrace($"{nameof(UserData)} after processing the Update: {{data}}", JsonSerializer.Serialize(userData));

        return userData;

        async Task<UserData?> GetUserDataIfExists() => await userDataBlobClient.ExistsAsync() ?
            (await userDataBlobClient.DownloadContentAsync()).Value.Content.ToObjectFromJson<UserData>() :
            null;

        async Task SendMessage(string responseText)
        {
            logger.LogTrace("Sending response to the user: \"{responseMessageText}\"", responseText);
            await response.WriteStringAsync(responseText);

            // bot client is null when running locally without the API key setting - we only want to return the text in the HTTP response
            if (botClient is { })
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, responseText, parseMode: ParseMode.Markdown);
            }
        }
    }
}
