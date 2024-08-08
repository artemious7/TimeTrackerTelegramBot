using System.Diagnostics.CodeAnalysis;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public interface IHandler
{
    public async Task<bool> TryHandle([AllowNull] string message, MessageSender SendMessage)
    {
        return await TryHandle((UserData?)null, SendMessage);
    }
    public async Task<bool> TryHandle(UserData? data, MessageSender messageSender)
    {
        return await TryHandle((string?)null, messageSender);
    }
}
