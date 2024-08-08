using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class WelcomeHandler : IHandler
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public async Task<bool> TryHandle(UserData data, MessageSender messageSender)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        return false;
    }
}