using Moq;
using SnowFlakeFactory.Service;
using SnowFlakeFactory.Interface;
using SnowFlakeFactory.Model;

namespace SnowFlakeFactoryTests.SnowFlakeServiceTests;

public class GetNextMillisecondsTests
{
    private const ulong _lastTimestamp = 89099756;
    Mock<IDateTimeProvider> _dateTimeProvider;

    public GetNextMillisecondsTests() =>
        _dateTimeProvider = new Mock<IDateTimeProvider>();

    [Fact]
    public void Should_Pass_In_GetNextMiliseconds_With_Do_Not_Enter_In_Loop()
    {
        DateTime getUtcNow = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 19, 20, 30, 30, 663);

        _dateTimeProvider.Setup(e => e.GetToday()).Returns(getToday);
        _dateTimeProvider.Setup(e => e.GetUtcNow()).Returns(getUtcNow);

        var snowFlakeModel = new SnowFlakeModel(_dateTimeProvider.Object) { Epoch = epoch };
        var sut = new SnowFlakeIdService(snowFlakeModel, _dateTimeProvider.Object);

        ulong result = sut.GetNextMilliseconds(_lastTimestamp);

        Assert.Equal((ulong)89099757, result);
        _dateTimeProvider.Verify(e => e.GetUtcNow(), Times.Once());
    }

    [Fact]
    public void Should_Pass_In_GetNextMiliseconds__Enter_In_Loop_To_Get_Next_Milisecond()
    {
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 19, 20, 30, 30, 663);

        _dateTimeProvider.Setup(e => e.GetToday()).Returns(getToday);

        MockSequence seq = new MockSequence();
        _dateTimeProvider.InSequence(seq)
            .Setup(e => e.GetUtcNow())
            .Returns(new DateTime(2012, 12, 20, 21, 15, 30, 419));

        _dateTimeProvider.InSequence(seq)
            .Setup(e => e.GetUtcNow())
            .Returns(new DateTime(2012, 12, 20, 21, 15, 30, 420));

        var snowFlakeModel = new SnowFlakeModel(_dateTimeProvider.Object) { Epoch = epoch };
        var sut = new SnowFlakeIdService(snowFlakeModel, _dateTimeProvider.Object);

        ulong result = sut.GetNextMilliseconds(_lastTimestamp);

        Assert.Equal((ulong)89099757, result);
        _dateTimeProvider.Verify(e => e.GetUtcNow(), Times.Exactly(2));
    }
}