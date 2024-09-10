namespace TimeTracker.Tests.Services;

public class ResponderFixture
{
    private readonly MessageSender messageSender = Substitute.For<MessageSender>();
    private readonly string message = "any message";

    [Fact]
    public async Task GivenEmptyListOfHandlers_WhenProcess_ThenDoNothing()
    {
        // Arrange
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
        List<IHandler> handlersInput = [.. Enumerable.Range(1, 10).Select(_ => Substitute.For<IHandler>())];
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

    [Fact]
    public async Task GivenListOfHandlers_WhenProcessAndAllAHandlerReturnsTrueAndNewData_ThenOnlyHandlersUpToThatOneAreCalledAndReturnsNewData()
    {
        // Arrange
        var trueHandler = Substitute.For<IHandler>();

        List<IHandler> handlersInput = [.. Enumerable.Range(1, 4).Select(_ => Substitute.For<IHandler>()), trueHandler, .. Enumerable.Range(1, 4).Select(_ => Substitute.For<IHandler>())];
        List<IHandler> calledHandlers = [];
        var originalData = new UserData(TimeSpan.FromHours(1), default, TimeSpan.FromHours(2));
        var newData = new UserData(TimeSpan.FromHours(2), default, TimeSpan.FromHours(3));
        trueHandler.TryHandle(default, default, messageSender)
            .ReturnsForAnyArgs(Task.FromResult(new HandleResult(true, newData)));
        handlersInput
            .ForEach(handler => handler
                .WhenForAnyArgs(r => r.TryHandle(default, default, messageSender))
                .Do(c => calledHandlers.Add(handler)));
        var sut = new Responder(message, originalData, messageSender, handlersInput);

        // Act
        var newDataReturned = await sut.Process();

        // Assert
        newDataReturned.Should().Be(newData, "user data should be changed by trueHandler");
        handlersInput.Take(5).ToList().ForEach(r => _ = r.Received(1).TryHandle(message, originalData, messageSender));
        handlersInput.Skip(5).ToList().ForEach(r => _ = r.DidNotReceiveWithAnyArgs().TryHandle(default, default, messageSender));
        calledHandlers.Should().Equal(handlersInput.Take(5));
    }
}
