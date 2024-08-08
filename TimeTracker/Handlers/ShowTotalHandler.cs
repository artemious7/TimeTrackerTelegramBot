﻿using TimeTracker.Handlers;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker;

public class ShowTotalHandler
{
    public async Task<HandleResult> TryHandle(string message, UserData? data, MessageSender SendMessage)
    {
        if (data is not null && IsCommand(ShowTotalCommand))
        {
            await ShowTotal();
            return new HandleResult(true, data);
        }

        return HandleResult.NotHandled;

        bool IsCommand(string command) => command.Equals(message, StringComparison.InvariantCultureIgnoreCase);

        async Task ShowTotal() => await SendMessage($"Total time recorded is {data.TimeString} since {data.Started:f}.");
    }

    private const string ShowTotalCommand = "/showTotal";
}