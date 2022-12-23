using Moq;
using SnowFlakeFactory.Service;
using SnowFlakeFactory.Interface;
using SnowFlakeFactory.Model;

namespace SnowFlakeFactoryTests.SnowFlakeServiceTests;

public class GetTimestampInMillisecondsTests
{
    Mock<IDateTimeProvider> _dateTimeProvider;

    public GetTimestampInMillisecondsTests() =>
        _dateTimeProvider = new Mock<IDateTimeProvider>();

    [Fact]
    public void Should_Subtract_Timestamps_And_Return_TotalMilliseconds()
    {
        DateTime getUtcNow = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 19, 20, 30, 30, 663);
         
        _dateTimeProvider.Setup(e => e.GetUtcNow()).Returns(getUtcNow);
        _dateTimeProvider.Setup(e => e.GetToday()).Returns(getToday);

        var snowFlakeModel = new SnowFlakeModel(_dateTimeProvider.Object, epoch);
        var sut = new SnowFlakeIdService(snowFlakeModel, _dateTimeProvider.Object);

        ulong result = sut.GetTimestampInMilliseconds();

        Assert.Equal((ulong)89099757, result);
    }
}