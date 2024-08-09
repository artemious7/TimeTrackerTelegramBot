using TimeTrackerBot.Services;

namespace TimeTrackerBot.TimeTracker;

public partial class Responder(UserData? data, global::TimeTracker.Services.Responder inner) : IResponder
{
    public async Task<UserData?> Process()
    {
        (var handled, var newData) = await inner.TryProcess();
        if (handled)
        {
            return newData;
        }
        else
        {
            return data;
        }

    }
}
