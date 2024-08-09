using TimeTracker.Services;

namespace TimeTracker.Handlers;

public interface IHandler
{
    Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage);
}
