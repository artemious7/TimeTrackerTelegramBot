using Microsoft.Extensions.Internal;
using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class WelcomeHandler(ISystemClock clock, IHelpResponder helpResponder) : IHandler
{
    public async Task<HandleResult> TryHandle(UserData? data, MessageSender messageSender)
    {
        if (data is not null)
        {
            return new HandleResult(false, data);
        }

        data = new UserData(default, Now, default);
        await helpResponder.Help(messageSender);
        return new HandleResult(true, data);
    }

    private DateTimeOffset Now => clock.UtcNow;
}
