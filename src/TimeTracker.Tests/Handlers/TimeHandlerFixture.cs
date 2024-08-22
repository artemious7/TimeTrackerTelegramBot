using System.Globalization;

namespace TimeTracker.Tests.Handlers;

public class TimeHandlerFixture
{
    private static readonly IFormatProvider formatProviderForDateTimeParsing = CultureInfo.InvariantCulture;
    private readonly TimeHandler sut = new(formatProviderForDateTimeParsing);
    private readonly MessageSender messageSender = Substitute.For<MessageSender>();
    private readonly DateTimeOffset Started = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero);

    [Theory]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("-1-05")]
    [InlineData("1:00-100:00")]
    [InlineData("24:00")]
    [InlineData("any other message")]
    public async Task GivenAnyOtherMessage_WhenTryHandle_ThenReturnsFalse(string message)
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
    [InlineData("1:05")]
    [InlineData("1-05")]
    public async Task GivenTimeMessage_WhenTryHandle_ThenAddsTimeAndResponds(string message)
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

    [Fact]
    public async Task GivenNegativeTimeMessage_WhenTryHandle_ThenSubtractsTimeAndResponds()
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
    [InlineData(" 0:00-1:05")]
    [InlineData(" 0:00-1:05 ")]
    [InlineData("0:00-1:05 ")]
    [InlineData("15:00-16:05")]
    [InlineData("17:30 - 18:35")]
    [InlineData("17:30 to 18:35")]
    [InlineData("17:30 -18:35")]
    [InlineData("17:30to 18:35")]
    [InlineData("17:30-18:35")]
    [InlineData("17:30–18:35")]
    [InlineData("17:30 18:35")]
    public async Task GivenTimeRangeMessage_WhenTryHandle_ThenAddsTimeAndResponds(string message)
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

    [Fact]
    public async Task GivenCloseToMaxCurrentTimeAndSmallTimeToAddMessage_WhenTryHandle_ThenSendsErrorResponse()
    {
        // Arrange
        var originalData = new UserData(Time: TimeSpan.FromHours(299) + TimeSpan.FromMinutes(1), Started: Started, PreviousTime: null);

        // Act
        var (handled, newData) = await sut.TryHandle("1:00", originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly($"You can't record more than 300 hours!");
        newData.Should().Be(originalData);
    }
     
    [Fact]
    public async Task GivenTimeRangeMessageWithStartGreaterThanEnd_WhenTryHandle_ThenSendsErrorResponse()
    {
        // Arrange
        var originalData = new UserData(Time: TimeSpan.FromHours(1), Started: Started, PreviousTime: null);

        // Act
        var (handled, newData) = await sut.TryHandle("14:00-13:00", originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        newData.Should().Be(originalData);
        await messageSender.SentOnly("End time must be greater than start time");
    }
}
