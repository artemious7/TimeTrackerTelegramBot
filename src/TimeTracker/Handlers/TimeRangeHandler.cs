using System.Diagnostics.CodeAnalysis;
using TimeTracker.Services;

namespace TimeTracker.Handlers;

public class TimeRangeHandler : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        if (data is { } && message is { })
        {
            if (TryParseTimeRange(out TimeRange timeRange))
            {
                await AddTimeRange(timeRange);
                return new HandleResult(true, data);
            }
            else if (TryParseTime(message, out TimeSpan time))
            {
                await AddTime(time);
                return new HandleResult(true, data);
            }
        }

        return new HandleResult(false, data);

        bool TryParseTimeRange(out TimeRange timeRange) => TimeRange.TryParse(message, null, out timeRange);

        async Task AddTimeRange(TimeRange timeRange)
        {
            if (timeRange.Duration <= TimeSpan.Zero)
                await SendMessage("End time must be greater than start time");
            else
                await AddTime(timeRange.Duration);
        }
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
            data = data with { Time = newTime, PreviousTime = data.Time };

            bool isAddingNegative = timeToAdd < TimeSpan.Zero;
            string verb = !isAddingNegative ? "Added" : "Subtracted";
            await SendMessage($"{verb} {UserData.FormatTime(absoluteTime)}. Total time recorded: {data.TimeString}");

            TimeSpan TrimToMinutes(TimeSpan time) => TimeSpan.FromMinutes(Math.Floor(time.TotalMinutes));
        }
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
    private static readonly TimeSpan MaxAccumulatedTime = TimeSpan.FromHours(300);
    internal struct TimeRange(TimeSpan Start, TimeSpan End) : IParsable<TimeRange>
    {
        private const string ExpectedFormats = $"Expected formats: \"1:30-2:30\" or \"1:30 to 2:30\"";
        private static readonly string[] separators = [" - ", " to ", " -", "to ", "-", "–", "—", " "];

        public readonly TimeSpan Duration => End - Start;

        public static TimeRange Parse(string s, IFormatProvider? provider) => TryParse(s, provider, out var result) ? result : throw new FormatException(ExpectedFormats);

        public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, out TimeRange result)
        {
            var parts = input?.Trim().Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (parts is not [{ } startString, { } endString]
                || !TryParseTime(startString, out TimeSpan start)
                || !TryParseTime(endString, out TimeSpan end))
            {
                result = default;
                return false;
            }
            result = new TimeRange(start, end);
            return true;
        }
    }
}