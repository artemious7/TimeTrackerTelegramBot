using FluentAssertions;
using Microsoft.Extensions.Internal;
using NSubstitute;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests;

public class WelcomeHandlerFixture
{
    private readonly ISystemClock clock;
    private readonly DateTimeOffset Now = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.FromHours(10));
    private readonly IHelpResponder helpResponder;
    private readonly WelcomeHandler sut;

    public WelcomeHandlerFixture()
    {
        clock = Substitute.For<ISystemClock>();
        clock.UtcNow.Returns(Now);
        helpResponder = Substitute.For<IHelpResponder>();
        sut = new(clock, helpResponder);
    }

    [Fact]
    public async Task GivenUserHasData_WhenTryHandle_ThenReturnsFalse()
    {
        // Arrange
        var messageSender = Substitute.For<MessageSender>();
        UserData? data = new UserData(default, default, default);

        // Act
        bool handled = await sut.TryHandle(data, messageSender);

        // Assert
        handled.Should().BeFalse();
        // TODO: to extension method
        await messageSender.DidNotReceiveWithAnyArgs().Invoke(Arg.Any<string>());
    }

    [Fact]
    public async Task GivenUserHasNoData_WhenTryHandle_ThenDataIsInitializedAndHelpMessageIsSentAndReturnsTrue()
    {
        // Arrange
        var messageSender = Substitute.For<MessageSender>();
        UserData? data = default;

        // Act
        (bool handled, UserData? userData) = await sut.TryHandle(data, messageSender);

        // Assert
        handled.Should().BeTrue();
        userData.Should().Be(new UserData(default, Now, default));
        await helpResponder.Received(1).Help(messageSender);
        helpResponder.ReceivedCalls().Should().ContainSingle();
        messageSender.ReceivedCalls().Should().BeEmpty();
    }
}
