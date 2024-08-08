using FluentAssertions;
using Microsoft.Extensions.Internal;
using NSubstitute;
using TimeTracker.Services;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests.Services;

public class ResponderFixture
{
    private readonly MessageSender messageSender = Substitute.For<MessageSender>();

    [Fact]
    public async Task GivenEmptyListOfHandlers_WhenProcess_ThenDoNothing()
    {
        // Arrange
        string message = "any message";
        List<IHandler> handlers = [];
        var originalData = new UserData(default, default, default);
        var sut = new Responder(message, originalData, messageSender, handlers);

        // Act
        var newData = await sut.Process();

        // Assert
        newData.Should().Be(originalData);
    }
}
