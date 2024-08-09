using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Services;

public class Responder(string message, UserData? data, MessageSender messageSender, IEnumerable<IHandler> handlers) : IResponder
{
    public async Task<(bool handled, UserData? data)> TryProcess()
    {
        foreach (var handler in handlers)
        {
            (bool handled, var newUserData) = await handler.TryHandle(message, data, messageSender);
            if (handled)
            {
                return (handled, newUserData);
            }
        }

        return (false, data);
    }

    public async Task<UserData?> Process()
    {
        return (await TryProcess()).data;
    }
}
