using FluentAssertions;
using NSubstitute;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests.Handlers;

public class UndoHandlerFixture
{
    private readonly UndoHandler sut = new();
    private readonly DateTimeOffset Started = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero);

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
    public async Task GivenUndoCommandAndNoData_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "/undo";
        var messageSender = Substitute.For<MessageSender>();

        // Act
        bool handled = await sut.TryHandle(message, null, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }

    [Fact]
    public async Task GivenUndoCommandAndUserDataHasPreviousTime_WhenTryHandle_ThenSendsUndoResponseAndRestoresPreviousTimeAndReturnsTrue()
    {
        // Arrange
        string message = "/undo";
        var messageSender = Substitute.For<MessageSender>();
        var originalData = new UserData(Time: TimeSpan.FromHours(2), Started: Started, PreviousTime: TimeSpan.FromHours(1));

        // Act
        (bool handled, var newData) = await sut.TryHandle(message, originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly($"Undone. Total time recorded: 1:00");
        newData.Should().Be(new UserData(Time: TimeSpan.FromHours(1), Started: originalData.Started, PreviousTime: null));
    }

    [Fact]
    public async Task GivenUndoCommandAndUserDataHasNoPreviousTime_WhenTryHandle_ThenSendsNothingToUndoResponseAndReturnsTrue()
    {
        // Arrange
        string message = "/undo";
        var messageSender = Substitute.For<MessageSender>();
        var originalData = new UserData(Time: TimeSpan.FromHours(2), Started: Started, PreviousTime: null);

        // Act
        (bool handled, var newData) = await sut.TryHandle(message, originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly("Nothing to undo");
        newData.Should().Be(originalData);
    }
}
