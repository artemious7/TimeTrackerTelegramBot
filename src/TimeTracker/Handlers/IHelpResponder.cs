using TimeTracker.Services;

namespace TimeTracker.Handlers;

public interface IHelpResponder
{
    Task Help(MessageSender messageSender);
}