using System.Text.Json.Serialization;

namespace TimeTracker.Data;

public record UserData(TimeSpan Time, DateTimeOffset Started, TimeSpan? PreviousTime)
{
    [JsonIgnore]
    public string TimeString => FormatTime(Time);
    public static string FormatTime(TimeSpan time) => $"{Math.Floor(time.TotalHours)}:{time:mm}";
}
