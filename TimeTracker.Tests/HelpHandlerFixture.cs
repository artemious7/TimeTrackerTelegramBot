using FluentAssertions;
using NSubstitute;
using TimeTrackerBot.Services;

namespace TimeTracker.Tests;

public class HelpHandlerFixture
{
    private readonly HelpHandler sut;

    public HelpHandlerFixture()
    {
        sut = new HelpHandler();
    }

    [Fact]
    public async Task GivenAnyOtherMessage_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "any other message";
        var messageSender = Substitute.For<MessageSender>();

        // Act
        bool handled = await sut.TryHandle(message, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<string>());
    }

    [Theory]
    [InlineData("/help")]
    [InlineData("/start")]
    public async Task GivenHelpCommand_WhenTryHandle_ThenSendsHelpMessageAndReturnsTrue(string message)
    {
        // Arrange
        var messageSender = Substitute.For<MessageSender>();

        // Act
        bool handled = await sut.TryHandle(message, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.Received(1).Invoke($"Send me the time, I will sum it up for you, e.g. `1:35` or `15:45 - 16:20` to add, or `-0:20` to subtract.  \r\nCommands:  \r\n /showTotal  \r\n /undo  \r\n /reset  \r\n /help");
    }
}
