
using SnowFlakeFactory.Interface;
using SnowFlakeFactory.Model;

namespace SnowFlakeFactory.Service;

public class SnowFlakeIdService
{
    private ulong _lastTimestamp;
    private SnowFlakeModel _snowFlakeModel;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SnowFlakeIdService(
        SnowFlakeModel snowFlakeModel,
        IDateTimeProvider dateTimeProvider) 
    {
        _dateTimeProvider = dateTimeProvider;
        _snowFlakeModel = snowFlakeModel;
    }
    
    public ulong GetNextMilliseconds(ulong lastTimestamp) {
        ulong actualTimestamp = GetTimestampInMilliseconds();

        while (actualTimestamp <= lastTimestamp)
            actualTimestamp = GetTimestampInMilliseconds(); 

        return actualTimestamp;
    }
        
    public ulong GetTimestampInMilliseconds() =>
            (ulong)(_dateTimeProvider.GetUtcNow() - _snowFlakeModel.Epoch).TotalMilliseconds;
    
}