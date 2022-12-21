using Moq;
using SnowFlakeFactory.Service;
using SnowFlakeFactory.Interface;

namespace SnowFlakeFactoryTests;

public class SnowFlakeIdServiceTests
{
    [Fact]
    public void Should_Subtract_Timestamps_And_Return_TotalMilliseconds()
    {
        DateTime getUtcNow = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 19, 20, 30, 30, 663);

        var dateTimePrivider = new Mock<IDateTimeProvider>();
        dateTimePrivider.Setup(e => e.GetUtcNow()).Returns(getUtcNow);
        dateTimePrivider.Setup(e => e.GetToday()).Returns(getToday);

        var sut = new SnowFlakeIdService(epoch, dateTimePrivider.Object);

        ulong result = sut.GetTimestampInMilliseconds();

        Assert.Equal((ulong)89099757, result);
    }

    [Fact]
    public void Should_Throw_Exception_If_Epoch_Is_In_Future()
    {
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 22, 20, 30, 30, 663);

        var dateTimePrivider = new Mock<IDateTimeProvider>();
        dateTimePrivider.Setup(e => e.GetToday()).Returns(getToday);

        Assert.Throws<ArgumentOutOfRangeException>(() => 
            new SnowFlakeIdService(epoch, dateTimePrivider.Object));
    }
}