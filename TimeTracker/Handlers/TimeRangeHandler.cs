using System.Diagnostics.CodeAnalysis;
using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Handlers;

public class TimeRangeHandler : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        return new HandleResult(false, data);
    }
}