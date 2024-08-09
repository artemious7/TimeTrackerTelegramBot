using TimeTrackerBot.TimeTracker;

namespace TimeTrackerBot.Services;

public interface IResponder
{
    Task<UserData?> Process();
}
