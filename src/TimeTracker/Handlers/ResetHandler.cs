using Microsoft.Extensions.Internal;
using TimeTracker.Services;

namespace TimeTracker.Handlers;

public class ResetHandler(ISystemClock clock) : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (data is { } && IsCommand(Commands.ResetCommand))
        {
            await Reset();
            return new HandleResult(true, data);
        }

        return new HandleResult(false, data);

        bool IsCommand(string command) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);

        async Task Reset()
        {
            data = data with { Time = default, Started = Now, PreviousTime = data.Time };
            await SendMessage($"Started over. Total time recorded: {data.TimeString}");
        }
    }

    private DateTimeOffset Now => clock.UtcNow;
}
