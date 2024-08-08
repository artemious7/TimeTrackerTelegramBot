using FluentAssertions;
using NSubstitute;
using System.Globalization;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests.Handlers;

public class ShowTotalHandlerFixture
{
    private readonly ShowTotalHandler sut;
    private readonly DateTimeOffset Now = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero);

    public ShowTotalHandlerFixture()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        sut = new ShowTotalHandler();
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

    [Fact]
    public async Task GivenShowTotalCommandAndNoData_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        string message = "/showtotal";
        var messageSender = Substitute.For<MessageSender>();

        // Act
        bool handled = await sut.TryHandle(message, (UserData?)null, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }

    [Fact]
    public async Task GivenShowTotalCommandLowerCase_WhenTryHandle_ThenSendsShowTotalResponseAndReturnsTrue()
    {
        // Arrange
        string message = "/showtotal";
        var messageSender = Substitute.For<MessageSender>();
        var originalData = new UserData(TimeSpan.FromHours(2), Now, TimeSpan.FromHours(1));

        // Act
        var (handled, newData) = await sut.TryHandle(message, originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        // TODO: ext method
        await messageSender.Received(1).Invoke($"Total time recorded is 2:00 since Friday, January 1, 2021 1:01 AM.");
        messageSender.ReceivedCalls().Should().ContainSingle();
        newData.Should().Be(originalData);
    }

}
