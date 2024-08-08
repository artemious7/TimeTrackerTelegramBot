using FluentAssertions;
using NSubstitute;
using TimeTrackerBot.Services;

namespace TimeTracker.Tests;

public static class MessageSenderExtensions
{
    public static async Task DidNotSendAnything(this MessageSender messageSender)
    {
        await messageSender.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<string>());
    }
}
