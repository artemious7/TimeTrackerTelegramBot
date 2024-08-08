using FluentAssertions;
using Microsoft.Extensions.Internal;
using NSubstitute;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests;

public class ResetHandlerFixture
{
    private readonly ResetHandler sut;

    public ResetHandlerFixture()
    {
        sut = new ResetHandler();
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

    //[Fact]
    //public async Task GivenResetCommand_WhenTryHandle_ThenSendsResetResponseAndReturnsEmptyDataAndReturnsFalse()
    //{
    //    // Arrange

    //}
}
