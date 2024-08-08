using TimeTrackerBot.Data;
using System.Text.Json.Serialization;

namespace TimeTrackerBot.TimeTracker;

public record UserData(TimeSpan Time, DateTimeOffset Started, TimeSpan? PreviousTime) : IUserData
{
    [JsonIgnore]
    public string TimeString => FormatTime(Time);
    public static string FormatTime(TimeSpan time) => $"{Math.Floor(time.TotalHours)}:{time:mm}";
}
