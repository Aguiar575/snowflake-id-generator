
using SnowFlakeFactory.Interface;

namespace SnowFlakeFactory.Service;

public class SnowFlakeIdService
{
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

    public SnowFlakeIdService(DateTime epoch, IDateTimeProvider dateTimeProvider) 
    {
        _dateTimeProvider = dateTimeProvider;
        Epoch = epoch;
    }

    public ulong GetTimestampInMilliseconds() =>
            (ulong)(_dateTimeProvider.GetUtcNow() - Epoch).TotalMilliseconds;
    
}