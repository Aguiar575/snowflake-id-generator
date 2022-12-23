using Moq;
using SnowFlakeFactory.Service;
using SnowFlakeFactory.Interface;
using SnowFlakeFactory.Model;

namespace SnowFlakeFactoryTests.SnowFlakeServiceTests;

public class CreateSnowflakeId
{
    private static readonly DateTime DefaultEpoch = new DateTime(1, 1, 1);
    Mock<IDateTimeProvider> _dateTimeProvider;

    public CreateSnowflakeId() =>
        _dateTimeProvider = new Mock<IDateTimeProvider>();

    [Theory]
    [InlineData("0001-1-1", 1UL)]
    [InlineData("0001-1-1 00:00:01", 4194304000UL)]
    [InlineData("0001-1-1 00:00:00.001", 4194304UL)]
    [InlineData("0001-1-2 00:00:00", 362387865600000UL)]
    [InlineData("9999-1-1 00:00:00", 13610765250948235264UL)]
    public void CreateSnowflakeId_With_Default_Datacenter_And_Worker_Id(string now,
                                                                        ulong expectedSnowflakeId)
    {   
        _dateTimeProvider.Setup(e => e.GetUtcNow()).Returns(DateTime.Parse(now));
        var model = new SnowFlakeModel(new DateTimeProvider(), DefaultEpoch);
        var snowflakeService = new SnowFlakeIdService(model, _dateTimeProvider.Object);

        var snowflakeId = snowflakeService.CreateSnowflakeId();

        Assert.Equal(expectedSnowflakeId, snowflakeId);
    }
}