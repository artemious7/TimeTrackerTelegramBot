﻿using Microsoft.Extensions.Internal;
using ReportCounterBot.Services;
using System.Diagnostics.CodeAnalysis;

namespace ReportCounterBot.ReportCounter;

public class Responder(string message, UserData? data, MessageSender SendMessage, ISystemClock clock) : IResponder
{
    public async Task<UserData?> Process()
    {
        if (data is null)
            await Welcome();
        else if (IsCommand(StartCommand) || IsCommand(HelpCommand))
            await Help();
        else if (IsCommand(ResetCommand))
            await Reset();
        else if (IsCommand(ShowTotalCommand))
            await ShowTotal();
        else if (IsCommand(UndoCommand))
            await Undo();
        else if (TryParseTimeRange(out TimeRange timeRange))
            await AddTimeRange(timeRange);
        else if (TryParseTime(message, out TimeSpan time))
            await AddTime(time);
        else
            await Error();
        return data;

        bool IsCommand(string command) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);

        async Task Welcome()
        {
            SaveData(new UserData(default, Now, default));
            await Help();
        }

        async Task Help() => await SendMessage($"Send me the time, I will sum it up for you, e.g. `1:35` or `15:45 - 16:20` to add, or `-0:20` to subtract.{LineBreak}Commands:{CommandListString}");

        async Task Reset()
        {
            SaveData(data with { Time = default, Started = Now, PreviousTime = data.Time });
            await SendMessage($"Started over. Total time recorded: {data.TimeString}");
        }

        async Task ShowTotal() => await SendMessage($"Total time recorded is {data.TimeString} since {data.Started:f}.");

        async Task Undo()
        {
            if (data.PreviousTime is not { } previousTime)
                await SendMessage("Nothing to undo");
            else
            {
                SaveData(data with { Time = previousTime, PreviousTime = default });
                await SendMessage($"Undone. Total time recorded: {data.TimeString}");
            }
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
    private const string LineBreak = "  \r\n";

    private DateTimeOffset Now => clock.UtcNow;
    private void SaveData(UserData newData) => data = newData;
    private static string CommandListString => string.Join("", CommandList.Select(command => $"{LineBreak} {command}"));

    private struct TimeRange(TimeSpan Start, TimeSpan End) : IParsable<TimeRange>
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
