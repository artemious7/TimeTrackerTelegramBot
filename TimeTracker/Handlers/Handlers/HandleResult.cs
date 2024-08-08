using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public record struct HandleResult(bool Handled, UserData? UserData)
{
    public static implicit operator bool(HandleResult result) => result.Handled;

}