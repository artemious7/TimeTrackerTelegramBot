﻿
namespace TimeTracker.Tests.Handlers;

public class TimeRangeHandlerFixture
{
    private readonly TimeRangeHandler sut = new();

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

    [Theory]
    [InlineData("1:05")]
    [InlineData("-1:00")]
    [InlineData("- 0:05")]
    public async Task GivenTimeMessage_WhenTryHandle_ThenReturnsFalse(string message)
    {
        // Arrange
        var messageSender = Substitute.For<MessageSender>();
        var data = new UserData(default, default, default);

        // Act
        bool handled = await sut.TryHandle(message, data, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }
}