namespace TimeTracker.Tests.Handlers;

public class UnknownCommandHandlerFixture
{
    [Fact]
    public async Task GivenAnyMessage_WhenTryHandle_ThenSendsUnknownCommandResponseAndReturnsTrue()
    {
        // Arrange
        var sut = new UnknownCommandHandler();
        var messageSender = Substitute.For<MessageSender>();
        var originalData = new UserData(default, default, default);

        // Act
        (bool handled, var newData) = await sut.TryHandle("any message", originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        newData.Should().Be(originalData);
        await messageSender.SentOnly("Oops, I didn't quite get that!");
    }
}
