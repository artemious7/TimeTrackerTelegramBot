using System.Diagnostics.CodeAnalysis;
using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public interface IHandler
{
    Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage);
}
