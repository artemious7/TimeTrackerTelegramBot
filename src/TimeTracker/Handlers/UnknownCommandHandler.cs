﻿using System.Diagnostics.CodeAnalysis;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Handlers;

public class UnknownCommandHandler : IHandler
{
    public async Task<HandleResult> TryHandle([AllowNull] string message, UserData? data, MessageSender SendMessage)
    {
        await SendMessage("Oops, I didn't quite get that!");
        return new(true, data);
    }
}