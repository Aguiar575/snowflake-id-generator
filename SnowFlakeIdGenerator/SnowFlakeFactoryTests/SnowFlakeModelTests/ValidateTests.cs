using Moq;
using SnowFlakeFactory.Model;
using SnowFlakeFactory.Interface;
using System.ComponentModel.DataAnnotations;

namespace SnowFlakeFactoryTests.SnowFlakeModelTests;

public class ValidateTests
{
    Mock<IDateTimeProvider> _dateTimeProvider;

    public ValidateTests() =>
        _dateTimeProvider = new Mock<IDateTimeProvider>();

    [Fact]
    public void Should_Throw_ValidationException_DatacenterId_Exceeds_MaxDatacenterId()
    {
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 20, 20, 30, 30, 663);
        _dateTimeProvider.Setup(e => e.GetToday()).Returns(getToday);

        var sut = new SnowFlakeModel(_dateTimeProvider.Object){
                Epoch = epoch,
                DatacenterId = 16
            };

        Assert.Throws<ValidationException>(() => sut.Validate());
    }

    [Fact]
    public void Should_Throw_ValidationException_WorkerId_Exceeds_MaxWorkerId()
    {
        DateTime getToday = new DateTime(2012, 12, 20, 21, 15, 30, 420);
        DateTime epoch = new DateTime(2012, 12, 20, 20, 30, 30, 663);
        _dateTimeProvider.Setup(e => e.GetToday()).Returns(getToday);

        var sut = new SnowFlakeModel(_dateTimeProvider.Object){
                Epoch = epoch,
                WorkerId = 64
            };

        Assert.Throws<ValidationException>(() => sut.Validate());
    }
}