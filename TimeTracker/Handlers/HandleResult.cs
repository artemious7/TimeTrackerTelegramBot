using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Handlers;

public record struct HandleResult(bool Handled, UserData? UserData)
{
    public static implicit operator bool(HandleResult result) => result.Handled;
    public static readonly HandleResult NotHandled = new HandleResult(false, null);
}