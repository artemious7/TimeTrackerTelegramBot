using System.Globalization;

namespace TimeTracker.Tests.Handlers;

public class ShowTotalHandlerFixture
{
    private readonly ShowTotalHandler sut;
    private readonly DateTimeOffset Now = new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero);
    private readonly MessageSender messageSender = Substitute.For<MessageSender>();

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

        // Act
        bool handled = await sut.TryHandle(message, null, messageSender);

        // Assert
        handled.Should().BeFalse();
        await messageSender.DidNotSendAnything();
    }

    [Theory]
    [InlineData("/showtotal")]
    [InlineData("/showTotal")]
    public async Task GivenShowTotalCommand_WhenTryHandle_ThenSendsShowTotalResponseAndReturnsTrue(string message)
    {
        // Arrange
        var originalData = new UserData(TimeSpan.FromHours(2), Now, TimeSpan.FromHours(1));

        // Act
        var (handled, newData) = await sut.TryHandle(message, originalData, messageSender);

        // Assert
        handled.Should().BeTrue();
        await messageSender.SentOnly("Total time recorded is 2:00 since Friday, January 1, 2021 1:01 AM.");
        newData.Should().Be(originalData);
    }

}
