using FluentAssertions;
using NSubstitute;
using TimeTrackerBot.Services;

namespace TimeTracker.Tests;

public class HelpHandlerFixture
{
    [Fact]
    public async Task GivenAnyMessage_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "any message";
        var messageSender = Substitute.For<MessageSender>();
        var handler = new HelpHandler();

        // Act
        bool handled = await handler.TryHandle(message, messageSender);

        // 
        handled.Should().BeFalse();
    }

    [Fact]
    public void GivenHelpCommand_WhenTryHandle_ThenSendsHelpMessageAndReturnsTrue()
    {

    }
}
