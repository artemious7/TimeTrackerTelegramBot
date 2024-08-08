using TimeTrackerBot.Services;

namespace TimeTracker;

public interface IHelpResponder
{
    Task Help(MessageSender messageSender);
}