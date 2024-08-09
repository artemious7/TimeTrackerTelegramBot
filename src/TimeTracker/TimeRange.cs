using System.Diagnostics.CodeAnalysis;

namespace TimeTrackerBot.TimeTracker;

public partial class Responder
{
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