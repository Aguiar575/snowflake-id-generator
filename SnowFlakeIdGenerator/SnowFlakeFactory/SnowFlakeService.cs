
using SnowFlakeFactory.Interface;

namespace SnowFlakeFactory.Service;

public class SnowFlakeIdService
{
    private ulong _lastTimestamp;
    private DateTime _epoch;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DateTime Epoch 
    { 
        get => _epoch; 
        private set {
            if(value > _dateTimeProvider.GetToday()) {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    "Epoch date must not be in the future.");
            }

            _epoch = value;
        }
    }

    public SnowFlakeIdService(
        DateTime epoch,
        IDateTimeProvider dateTimeProvider,
        ulong lastTimeStamp) 
    {
        _dateTimeProvider = dateTimeProvider;
        _lastTimestamp = lastTimeStamp;
        Epoch = epoch;
    }
    
    public ulong GetNextMilliseconds() {
        ulong actualTimestamp = GetTimestampInMilliseconds();

        while (actualTimestamp <= _lastTimestamp)
            actualTimestamp = GetTimestampInMilliseconds(); 

        return actualTimestamp;
    }
        
    public ulong GetTimestampInMilliseconds() =>
            (ulong)(_dateTimeProvider.GetUtcNow() - Epoch).TotalMilliseconds;
    
}