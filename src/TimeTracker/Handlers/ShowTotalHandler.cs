using TimeTracker.Services;

namespace TimeTracker.Handlers;

public class ShowTotalHandler : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (data is not null && Commands.IsCommand(Commands.ShowTotalCommand, message))
        {
            await ShowTotal();
            return new HandleResult(true, data);
        }

        return HandleResult.NotHandled;

        async Task ShowTotal() => await SendMessage($"Total time recorded is {data.TimeString} since {data.Started:f}.");
    }
}