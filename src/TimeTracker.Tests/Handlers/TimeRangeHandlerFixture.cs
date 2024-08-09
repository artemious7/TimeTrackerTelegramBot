
namespace TimeTracker.Tests.Handlers;

public class TimeRangeHandlerFixture
{
    private readonly TimeRangeHandler sut = new();
    private readonly MessageSender messageSender = Substitute.For<MessageSender>();
    private readonly DateTimeOffset Started = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero);

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

    [Fact]
    public async Task GivenTimeMessage_WhenTryHandle_ThenAddsTimeAndRespondsAndReturnsTrue()
    {
        // Arrange
        var originalData = new UserData(Time: TimeSpan.FromHours(4), Started: Started, PreviousTime: null);

        // Act
        var (handled, newData) = await sut.TryHandle("1:05", originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly($"Added 1:05. Total time recorded: 5:05");
        newData.Should().Be(new UserData(Time: TimeSpan.FromHours(5) + TimeSpan.FromMinutes(5), Started: Started, PreviousTime: originalData.Time));
    }

    [Fact]
    public async Task GivenNegativeTimeMessage_WhenTryHandle_ThenSubtractsTimeAndRespondsAndReturnsTrue()
    {
        // Arrange
        var originalData = new UserData(Time: TimeSpan.FromHours(4), Started: Started, PreviousTime: null);

        // Act
        var (handled, newData) = await sut.TryHandle("-1:05", originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly($"Subtracted 1:05. Total time recorded: 2:55");
        newData.Should().Be(new UserData(Time: TimeSpan.FromHours(2) + TimeSpan.FromMinutes(55), Started: Started, PreviousTime: originalData.Time));
    }

    [Theory]
    [InlineData("0:00-1:05")]
    [InlineData("15:00-16:05")]
    [InlineData("17:30 - 18:35")]
    public async Task GivenTimeRangeMessage_WhenTryHandle_ThenAddsTimeAndRespondsAndReturnsTrue(string message)
    {
        // Arrange
        var originalData = new UserData(Time: TimeSpan.FromHours(4), Started: Started, PreviousTime: null);

        // Act
        var (handled, newData) = await sut.TryHandle(message, originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly($"Added 1:05. Total time recorded: 5:05");
        newData.Should().Be(new UserData(Time: TimeSpan.FromHours(5) + TimeSpan.FromMinutes(5), Started: Started, PreviousTime: originalData.Time));
    }
}
