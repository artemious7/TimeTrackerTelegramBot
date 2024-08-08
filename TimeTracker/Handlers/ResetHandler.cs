using Microsoft.Extensions.Internal;
using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class ResetHandler : IHandler
{
    public async Task<HandleResult> TryHandle(string message, UserData? data, MessageSender messageSender)
    {
        return new HandleResult(false, data);
    }
}
