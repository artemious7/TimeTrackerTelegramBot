using Microsoft.Extensions.Internal;
using TimeTracker.Services;

namespace TimeTracker.Handlers;

public class WelcomeHandler(ISystemClock clock, IHelpResponder helpResponder) : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender messageSender)
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
