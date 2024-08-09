using System.Diagnostics.CodeAnalysis;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Handlers;

public interface IHandler
{
    Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage);
}
