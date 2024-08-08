using FluentAssertions;
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

    [Fact]
    public async Task GivenListOfHandlers_WhenProcessAndAllHandlersReturnFalse_ThenAllHandlersCalled()
    {
        // Arrange
        string message = "any message";
        List<IHandler> handlers = [.. Enumerable.Range(1, 10).Select(r => Substitute.For<IHandler>())];
        var originalData = new UserData(TimeSpan.FromHours(1), default, TimeSpan.FromHours(2));
        var sut = new Responder(message, originalData, messageSender, handlers);

        // Act
        var newData = await sut.Process();

        // Assert
        newData.Should().Be(originalData, "user data should not be changed unless changed by handlers");
        handlers.ForEach(r => _ = r.Received(1).TryHandle(message, messageSender));
    }
}
