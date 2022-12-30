using SnowFlakeFactory.Model;
using SnowFlakeFactory.Interface;

namespace SnowFlakeFactory.Service;

public class SnowFlakeIdService
{
    private const int SEQUENCE_BITS = 12;
    private readonly int _workerIdShift;
    private readonly int _datacenterIdShift;
    private readonly int _timestampLeftShift;
    private readonly ulong _sequenceMask;
    private ulong _lastTimestamp;
    private ulong _sequence;

    private SnowFlakeModel _snowFlakeModel;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SnowFlakeIdService(
        SnowFlakeModel snowFlakeModel,
        IDateTimeProvider dateTimeProvider) 
    {
        snowFlakeModel.Validate();

        _dateTimeProvider = dateTimeProvider;
        _snowFlakeModel = snowFlakeModel;

        _workerIdShift = SEQUENCE_BITS;
        _datacenterIdShift = SEQUENCE_BITS + _snowFlakeModel.WorkerIdBits;
        _timestampLeftShift = SEQUENCE_BITS + _snowFlakeModel.WorkerIdBits 
                                            + _snowFlakeModel.DatacenterIdBits;

        _sequenceMask = -1L ^ (-1L << SEQUENCE_BITS);
    }
    
    public ulong CreateSnowflakeId()
    {
        var timestamp = GetTimestampInMilliseconds();

        if (timestamp < _lastTimestamp)
            throw new InvalidOperationException(
                $"Clock moved backwards. Refusing to generate ID for {_lastTimestamp - timestamp} milliseconds.");
 
        if (timestamp == _lastTimestamp)
        {
            _sequence = (_sequence + 1UL) & _sequenceMask;
            if (_sequence == 0UL)
                timestamp = GetNextMilliseconds(_lastTimestamp);
        }
        else
        {
            _sequence = 0UL;
        }

        _lastTimestamp = timestamp;

        return (timestamp << _timestampLeftShift) |
            (_snowFlakeModel.DatacenterId << _datacenterIdShift) |
            (_snowFlakeModel.WorkerId << _workerIdShift) |
            _sequence;
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