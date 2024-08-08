using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Services;

public class Responder(string message, UserData? data, MessageSender messageSender, IEnumerable<IHandler> handlers)
{
    public async Task<UserData?> Process()
    {
        foreach (var handler in handlers)
        {
            (bool handled, var newUserData) = await handler.TryHandle(message, data, messageSender);
            if (handled)
            {
                return newUserData;
            }
        }

        return data;
    }
}
