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
    public async Task GivenListOfHandlers_WhenProcessAndAllHandlersReturnFalse_ThenAllHandlersCalledInOrder()
    {
        // Arrange
        string message = "any message";
        List<IHandler> handlersInput = [.. Enumerable.Range(1, 10).Select(r => Substitute.For<IHandler>())];
        List<IHandler> calledHandlers = [];
        handlersInput.ForEach(handler => handler
            .WhenForAnyArgs(r => r.TryHandle(default, default, messageSender))
            .Do(c => calledHandlers.Add(handler)));
        var originalData = new UserData(TimeSpan.FromHours(1), default, TimeSpan.FromHours(2));
        var sut = new Responder(message, originalData, messageSender, handlersInput);

        // Act
        var newData = await sut.Process();

        // Assert
        newData.Should().Be(originalData, "user data should not be changed unless changed by handlersInput");
        handlersInput.ForEach(r => _ = r.Received(1).TryHandle(message, originalData, messageSender));
        calledHandlers.Should().Equal(handlersInput);
    }
}
