using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class WelcomeHandler : IHandler
{
    public async Task<bool> TryHandle(UserData data, MessageSender messageSender)
    {
        return false;
    }
}