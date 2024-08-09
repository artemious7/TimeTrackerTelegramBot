using System.Diagnostics.CodeAnalysis;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Handlers;

public class UndoHandler : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (data is { } && IsCommand(UndoCommand))
        {
            await Undo();
            return new HandleResult(true, data);
        }

        return new HandleResult(false, data);

        bool IsCommand(string command) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);

        async Task Undo()
        {
            if (data.PreviousTime is not { } previousTime)
                await SendMessage("Nothing to undo");
            else
            {
                data = data with { Time = previousTime, PreviousTime = null };
                await SendMessage($"Undone. Total time recorded: {data.TimeString}");
            }
        }
    }

    private const string UndoCommand = "/undo";
}
