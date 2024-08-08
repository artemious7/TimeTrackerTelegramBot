using FluentAssertions;
using NSubstitute;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests.Handlers;

public class ShowTotalHandlerFixture
{
    private readonly ShowTotalHandler sut;

    public ShowTotalHandlerFixture()
    {
        sut = new ShowTotalHandler();
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
}
