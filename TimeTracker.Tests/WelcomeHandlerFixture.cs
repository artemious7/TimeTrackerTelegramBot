using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerBot.Services;
using TimeTrackerBot.TimeTracker;

namespace TimeTracker.Tests;

public class WelcomeHandlerFixture
{
    private readonly WelcomeHandler sut = new();

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

    //[Fact]
    //public async Task GivenUserHasNoData_WhenTryHandle_ThenDataIsInitializedAndHelpMessageIsSentAndReturnsTrue()
    //{
    //    // Arrange
    //    var messageSender = Substitute.For<MessageSender>();
    //    UserData? data = default;

    //    // Act
    //    bool handled = await sut.TryHandle(data, messageSender);

    //    // Assert
    //}
}
