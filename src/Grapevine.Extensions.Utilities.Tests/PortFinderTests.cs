namespace Grapevine.Extensions.Utilities.Tests;

public class PortFinderTests
{
    [Theory]
    [InlineData(1, true)]
    [InlineData(65535, true)]
    [InlineData(0, false)]
    [InlineData(65536, false)]
    [InlineData(-1, false)]
    public void IsValidPortNumber_WorksAsExpected(int value, bool expected)
    {
        value.IsValidPortNumber().ShouldBe(expected);
    }

    [Fact]
    public void Constants_AreCorrect()
    {
        PortFinder.FirstPort.ShouldBe(1);
        PortFinder.LastPort.ShouldBe(65535);
        PortFinder.FirstServicePort.ShouldBe(1024);
        PortFinder.LastServicePort.ShouldBe(49151);
    }

    [Fact]
    public void TryFindAvailablePort_ReturnsTrue_WhenPortAvailable()
    {
        var result = PortFinder.TryFindAvailablePort(out var port, 30000, 30010);
        result.ShouldBeTrue();
        port.ShouldBeInRange(30000, 30010);
        port.IsValidPortNumber().ShouldBeTrue();
    }

    [Fact]
    public void TryFindAvailablePort_Throws_WhenRangeInvalid()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            PortFinder.TryFindAvailablePort(out _, 0, 5);
        });
    }

    [Fact]
    public void FindAvailablePort_Throws_WhenStartOutOfRange()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            PortFinder.FindAvailablePort(0, 1024);
        });
    }

    [Fact]
    public void FindAvailablePort_Throws_WhenEndOutOfRange()
    {
        Should.Throw<ArgumentOutOfRangeException>(() =>
        {
            PortFinder.FindAvailablePort(1024, 70000);
        });
    }

    [Fact]
    public void FindAvailablePort_Throws_WhenStartGreaterThanEnd()
    {
        Should.Throw<ArgumentException>(() =>
        {
            PortFinder.FindAvailablePort(2000, 1000);
        });
    }

    [Fact]
    public void FindNextLocalOpenPort_ReturnsPortInServiceRange()
    {
        var port = PortFinder.FindNextLocalOpenPort();
        port.ShouldBeInRange(PortFinder.FirstServicePort, PortFinder.LastServicePort);
    }

    [Fact]
    public void FindLastLocalOpenPort_ReturnsPortInServiceRange()
    {
        var port = PortFinder.FindLastLocalOpenPort();
        port.ShouldBeInRange(PortFinder.FirstServicePort, PortFinder.LastServicePort);
    }
}
