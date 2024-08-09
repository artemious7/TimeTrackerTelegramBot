using TimeTrackerBot.Services;

namespace TimeTrackerBot.TimeTracker;

public partial class Responder(string message, UserData? data, MessageSender SendMessage, global::TimeTracker.Services.Responder inner) : IResponder
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
            if (TryParseTimeRange(out TimeRange timeRange))
                await AddTimeRange(timeRange);
            else if (TryParseTime(message, out TimeSpan time))
                await AddTime(time);
            else
                await Error();
            return data;
        }

        async Task AddTimeRange(TimeRange timeRange)
        {
            if (timeRange.Duration <= TimeSpan.Zero)
                await SendMessage("End time must be greater than start time");
            else
                await AddTime(timeRange.Duration);
        }

        bool TryParseTimeRange(out TimeRange timeRange) => TimeRange.TryParse(message, null, out timeRange);

        async Task AddTime(TimeSpan timeToAdd)
        {
            timeToAdd = TrimToMinutes(timeToAdd);
            TimeSpan absoluteTime = TimeSpan.FromTicks(Math.Abs(timeToAdd.Ticks));
            TimeSpan newTime = data.Time + timeToAdd;
            // not checking if it's negative to allow for debt carryover
            if (absoluteTime > MaxAccumulatedTime)
            {
                await SendMessage($"You can't record more than {MaxAccumulatedTime.TotalHours:N0} hours!");
                return;
            }
            SaveData(data with { Time = newTime, PreviousTime = data.Time });

            bool isAddingNegative = timeToAdd < TimeSpan.Zero;
            string verb = !isAddingNegative ? "Added" : "Subtracted";
            await SendMessage($"{verb} {UserData.FormatTime(absoluteTime)}. Total time recorded: {data.TimeString}");

            TimeSpan TrimToMinutes(TimeSpan time) => TimeSpan.FromMinutes(Math.Floor(time.TotalMinutes));
        }

        async Task Error() => await SendMessage("Oops, I didn't quite get that!");
    }

    private static bool TryParseTime(string message, out TimeSpan time)
    {
        message = message.Replace(" ", "").Trim().Replace('.', ':');
        // treat '-' as ':' unless it's negative time since we don't want to parse "-1-30" because of ambiguity but we do want to treat "1-30" as "1:30"
        if (!message.StartsWith('-'))
        {
            message = message.Replace('-', ':');
        }
        return TimeSpan.TryParse(message, out time) &&
            // can't be an integer, otherwise it would be interpreted as days which probably wasn't intended
            !int.TryParse(message, out _);
    }

    private const string StartCommand = "/start";
    private const string HelpCommand = "/help";
    private const string ResetCommand = "/reset";
    private const string ShowTotalCommand = "/showTotal";
    private const string UndoCommand = "/undo";
    private static readonly string[] CommandList = [ShowTotalCommand, UndoCommand, ResetCommand, HelpCommand];
    private static readonly TimeSpan MaxAccumulatedTime = TimeSpan.FromHours(300);

    private void SaveData(UserData newData) => data = newData;
}
