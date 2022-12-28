using Moq;
using SnowFlakeFactory.Service;
using SnowFlakeFactory.Interface;
using SnowFlakeFactory.Model;

namespace SnowFlakeFactoryTests.SnowFlakeServiceTests;

public class CreateSnowflakeIdTests
{
    private static readonly DateTime DefaultEpoch = new DateTime(1, 1, 1);
    Mock<IDateTimeProvider> _dateTimeProvider;

    public CreateSnowflakeIdTests() =>
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
        var model = new SnowFlakeModel(new DateTimeProvider()) { Epoch = DefaultEpoch };
        var snowflakeService = new SnowFlakeIdService(model, _dateTimeProvider.Object);

        var snowflakeId = snowflakeService.CreateSnowflakeId();

        Assert.Equal(expectedSnowflakeId, snowflakeId);
    }

    [Fact]
    public void CreateSnowflakeId_With_Two_Different_DataCenters_Produce_Different_Ids()
    {
        _dateTimeProvider.Setup(e => e.GetUtcNow())
                         .Returns(new DateTime(2000, 1, 1));
        var modelOne = new SnowFlakeModel(new DateTimeProvider())
        {
            Epoch = DefaultEpoch,
            DatacenterId = 1
        };
        var modelTwo = new SnowFlakeModel(new DateTimeProvider())
        {
            Epoch = DefaultEpoch,
            DatacenterId = 2
        };
        var snowflakeService1 = new SnowFlakeIdService(modelOne, _dateTimeProvider.Object);
        var snowflakeService2 = new SnowFlakeIdService(modelTwo, _dateTimeProvider.Object);
        var snowflakeId1 = snowflakeService1.CreateSnowflakeId();
        var snowflakeId2 = snowflakeService2.CreateSnowflakeId();

        Assert.NotEqual(snowflakeId1, snowflakeId2);
    }

    [Fact]
    public void CreateSnowflakeId_With_Two_Different_Workers_Produce_Different_Ids()
    {
        _dateTimeProvider.Setup(e => e.GetUtcNow())
                         .Returns(new DateTime(2000, 1, 1));
        var modelOne = new SnowFlakeModel(new DateTimeProvider())
        {
            Epoch = DefaultEpoch,
            WorkerId = 1
        };
        var modelTwo = new SnowFlakeModel(new DateTimeProvider())
        {
            Epoch = DefaultEpoch,
            WorkerId = 2
        };
        var snowflakeService1 = new SnowFlakeIdService(modelOne, _dateTimeProvider.Object);
        var snowflakeService2 = new SnowFlakeIdService(modelTwo, _dateTimeProvider.Object);
        var snowflakeId1 = snowflakeService1.CreateSnowflakeId();
        var snowflakeId2 = snowflakeService2.CreateSnowflakeId();

        Assert.NotEqual(snowflakeId1, snowflakeId2);
    }

    [Fact]
    public void CreateSnowflakeId_Default_Should_Not_Generate_Zero_Or_Negative_Ids()
    {
        var model = new SnowFlakeModel(new DateTimeProvider()) { Epoch = DefaultEpoch };
        var snowflakeService = new SnowFlakeIdService(model, new DateTimeProvider());
        for (var i = 0; i < 1_000_000; i++)
        {
            var snowflakeId = snowflakeService.CreateSnowflakeId();
            Assert.False(snowflakeId <= 0);
        }
    }

    [Fact]
    public void CreateSnowflakeId_Default_Should_Generate_Sequential_Ids()
    {
        var model = new SnowFlakeModel(new DateTimeProvider()) { Epoch = DefaultEpoch };
        var snowflakeService = new SnowFlakeIdService(model, new DateTimeProvider());

        ulong? snowflakeId = null;
        ulong? lastSnowflakeId = null;
        for (var i = 0; i < 1_000_000; i++)
        {
            lastSnowflakeId = snowflakeId;
            snowflakeId = snowflakeService.CreateSnowflakeId();

            if (lastSnowflakeId.HasValue)
            {
                Assert.True(snowflakeId.Value > lastSnowflakeId.Value);
            }
        }
    }

    [Fact]
    public void CreateSnowflake_ClockMovesBack_ThrowsInvalidOperationException()
    {
        MockSequence seq = new MockSequence();
        _dateTimeProvider.InSequence(seq)
            .Setup(e => e.GetUtcNow())
            .Returns(new DateTime(2000, 1, 1));

        _dateTimeProvider.InSequence(seq)
            .Setup(e => e.GetUtcNow())
            .Returns(new DateTime(2000, 1, 1).AddMilliseconds(-1));

        var model = new SnowFlakeModel(new DateTimeProvider()) { Epoch = DefaultEpoch };
        var snowflakeService = new SnowFlakeIdService(model, _dateTimeProvider.Object);
        snowflakeService.CreateSnowflakeId();

        Assert.Throws<InvalidOperationException>(() => snowflakeService.CreateSnowflakeId());
    }
}