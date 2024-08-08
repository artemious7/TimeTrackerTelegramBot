using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class ShowTotalHandler
{
    public ShowTotalHandler()
    {

    }

    public async Task<HandleResult> TryHandle(string message, UserData data, MessageSender messageSender)
    {
        return HandleResult.NotHandled;
    }
}