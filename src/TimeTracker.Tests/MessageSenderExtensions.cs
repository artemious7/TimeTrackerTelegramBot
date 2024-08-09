using FluentAssertions;
using NSubstitute;
using TimeTrackerBot.Services;

namespace TimeTracker.Tests;

public static class MessageSenderExtensions
{
    public static async Task DidNotSendAnything(this MessageSender messageSender)
    {
        ArgumentNullException.ThrowIfNull(messageSender);

        await messageSender.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<string>());
    }

    public static async Task SentOnly(this MessageSender messageSender, string message)
    {
        ArgumentNullException.ThrowIfNull(messageSender);

        await messageSender.Received(1).Invoke(message);
        messageSender.ReceivedCalls().Should().ContainSingle();
    }
}
