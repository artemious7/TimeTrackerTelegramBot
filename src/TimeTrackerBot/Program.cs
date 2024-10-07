using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Internal;
using TimeTracker.Handlers;
using TimeTracker.Services;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication(workerApp =>
    {
        workerApp.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<ISystemClock, SystemClock>();

        services.AddSingleton<IHandler, WelcomeHandler>();
        services.AddSingleton<IHandler, ShowTotalHandler>();
        services.AddSingleton<IHandler, HelpHandler>();
        services.AddSingleton<IHelpResponder, HelpHandler>();
        services.AddSingleton<IHandler, ResetHandler>();
        services.AddSingleton<IHandler, UndoHandler>();
        services.AddSingleton<IHandler, TimeHandler>();
        services.AddSingleton<IFormatProvider>(System.Globalization.CultureInfo.InvariantCulture);
        services.AddSingleton<IHandler, UnknownCommandHandler>(); // must be the last one registered

        services.AddSingleton(sp => new ResponderFactory((message, data, messageSender) =>
            new TimeTracker.Services.Responder(message, data, messageSender, sp.GetServices<IHandler>())));
    })
    .Build();

InitializeDataBlobStorage();
host.Run();

void InitializeDataBlobStorage()
{
    var configuration = host.Services.GetRequiredService<IConfiguration>();
    string azureStorageConnectionString = GetRequiredSetting(SettingsKeys.AzureWebJobsStorage);
    string dataContainerName = GetRequiredSetting(SettingsKeys.UserDataBlobContainerName);
    var blobServiceClient = new BlobServiceClient(azureStorageConnectionString);

    blobServiceClient
        .GetBlobContainerClient(dataContainerName)
        .CreateIfNotExists();

    string GetRequiredSetting(string name) =>
        configuration[name] ??
        throw new InvalidOperationException($"The required setting '{name}' is not set. If running in Azure, set it in the Function app settings. If running locally, set it the local.settings.json file.");
}
