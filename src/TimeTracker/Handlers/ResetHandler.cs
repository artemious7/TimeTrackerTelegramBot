using Microsoft.Extensions.Internal;
using TimeTracker.Services;

namespace TimeTracker.Handlers;

public class ResetHandler(ISystemClock clock) : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (data is { } && Commands.IsCommand(Commands.ResetCommand, message))
        {
            await Reset();
            return new HandleResult(true, data);
        }

        return new HandleResult(false, data);

        async Task Reset()
        {
            data = data with { Time = default, Started = clock.UtcNow, PreviousTime = data.Time };
            await SendMessage($"Started over. Total time recorded: {data.TimeString}");
        }
    }
}
