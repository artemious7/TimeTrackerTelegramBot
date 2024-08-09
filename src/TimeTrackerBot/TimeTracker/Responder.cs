using TimeTrackerBot.Services;

namespace TimeTrackerBot.TimeTracker;

public partial class Responder(UserData? data, MessageSender SendMessage, global::TimeTracker.Services.Responder inner) : IResponder
{
    public async Task<UserData?> Process()
    {
        (var handled, var newData) = await inner.TryProcess();
        if (handled)
        {
            return newData;
        }
        else if (data is null)
        {
            throw new InvalidOperationException("The case where data is null should have been handled by TimeTracker.Services.Responder.TryProcess()");
        }
        else
        {
            await Error();
            return data;
        }

        async Task Error() => await SendMessage("Oops, I didn't quite get that!");
    }
}
