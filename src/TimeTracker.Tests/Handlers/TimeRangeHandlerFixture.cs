
namespace TimeTracker.Tests.Handlers;

public class TimeRangeHandlerFixture
{
    private readonly TimeRangeHandler sut = new();
    private readonly MessageSender messageSender = Substitute.For<MessageSender>();

    [Fact]
    public async Task GivenAnyOtherMessage_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "any other message";
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
        var data = new UserData(default, default, default);

        // Act
        bool handled = await sut.TryHandle(message, data, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }

    [Theory]
    [InlineData("0:00-1:05")]
    [InlineData("15:00-16:05")]
    [InlineData("17:30 - 18:35")]
    public async Task GivenTimeRangeMessage_WhenTryHandle_ThenAddsTimeAndRespondsAndReturnsTrue(string message)
    {
        // Arrange
        var originalData = new UserData(Time: TimeSpan.FromHours(4), Started: default, PreviousTime: null);

        // Act
        var (handled, newData) = await sut.TryHandle(message, originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly($"Added 1:05. Total time recorded: 5:05");
        newData.Should().Be(new UserData(Time: TimeSpan.FromHours(5) + TimeSpan.FromMinutes(5), Started: default, PreviousTime: originalData.Time));
    }
}
