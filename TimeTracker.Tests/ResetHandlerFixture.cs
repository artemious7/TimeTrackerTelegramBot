using FluentAssertions;
using Microsoft.Extensions.Internal;
using NSubstitute;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests;

public class ResetHandlerFixture
{
    private readonly ResetHandler sut;
    private readonly DateTimeOffset Now = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero);
    private readonly ISystemClock clock = Substitute.For<ISystemClock>();

    public ResetHandlerFixture()
    {
        clock.UtcNow.Returns(Now);
        sut = new ResetHandler(clock);
    }

    [Fact]
    public async Task GivenAnyOtherMessage_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "any other message";
        var messageSender = Substitute.For<MessageSender>();
        var data = new UserData(default, default, default);

        // Act
        bool handled = await sut.TryHandle(message, data, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }

    [Fact]
    public async Task GivenResetCommandAndNoData_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "any other message";
        var messageSender = Substitute.For<MessageSender>();

        // Act
        bool handled = await sut.TryHandle(message, (UserData?)null, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }

    [Fact]
    public async Task GivenResetCommand_WhenTryHandle_ThenSendsResetResponseAndReturnsEmptyDataWithCurrentTimeAndPreviousTimeAndReturnsTrue()
    {
        // Arrange
        string message = "/reset";
        var messageSender = Substitute.For<MessageSender>();
        var data = new UserData(TimeSpan.FromHours(2), Now - TimeSpan.FromHours(22), TimeSpan.FromHours(1));

        // Act
        (bool handled, data) = await sut.TryHandle(message, data, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.Received(1).Invoke($"Started over. Total time recorded: 0:00");
        messageSender.ReceivedCalls().Should().ContainSingle();
        data.Should().Be(new UserData(TimeSpan.Zero, Now, TimeSpan.FromHours(2)));
    }
}
