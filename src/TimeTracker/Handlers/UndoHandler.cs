using TimeTracker.Services;

namespace TimeTracker.Handlers;

public class UndoHandler : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (data is { } && Commands.IsCommand(Commands.UndoCommand, message))
        {
            await Undo();
            return new HandleResult(true, data);
        }

        return new HandleResult(false, data);

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
}
