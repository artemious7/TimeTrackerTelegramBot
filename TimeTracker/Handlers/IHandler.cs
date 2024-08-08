using System.Diagnostics.CodeAnalysis;
using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public interface IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (message == null)
            return await TryHandle(data, SendMessage);
        else if (data == null)
            return await TryHandle(message, SendMessage);
        else
            throw new InvalidOperationException($"At least message or user data has to be not null.");
    }

    public async Task<HandleResult> TryHandle([AllowNull] string message, MessageSender SendMessage)
    {
        return await TryHandle(default, default, SendMessage);
    }

    public async Task<HandleResult> TryHandle(UserData? data, MessageSender messageSender)
    {
        return await TryHandle(default, default, messageSender);
    }
}
