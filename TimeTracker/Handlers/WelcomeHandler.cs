using Microsoft.Extensions.Internal;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class WelcomeHandler(ISystemClock clock, IHelpResponder helpResponder) : IHandler
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public async Task<HandleResult> TryHandle(UserData? data, MessageSender messageSender)
    {
        if (data is null)
        {
            SaveData(new UserData(default, Now, default));
            await helpResponder.Help(messageSender);
            return new HandleResult(true, data);
        }
        return new HandleResult(false, data);

        void SaveData(UserData newData) => data = newData;
    }

    private DateTimeOffset Now => clock.UtcNow;
}
