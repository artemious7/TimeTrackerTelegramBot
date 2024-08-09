using TimeTrackerBot.Services;

namespace TimeTracker.Handlers;

public interface IHelpResponder
{
    Task Help(MessageSender messageSender);
}