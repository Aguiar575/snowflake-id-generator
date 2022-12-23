using SnowFlakeFactory.Interface;

namespace SnowFlakeFactory.Model;

public class SnowFlakeModel
{
    private const int _maxFreeBits = 10;
    private int _datacenterIdBits = 4;
    private int _workerIdBits = 6;
    private DateTime _epoch;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ulong WorkerId { get; init; } = 0UL;
    public ulong DatacenterId { get; init; } = 0UL;

    public DateTime Epoch 
    { 
        get => _epoch; 
        private set {
            if(value > _dateTimeProvider.GetToday()) {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    value,
                    $"Epoch date must not be in the future.");
            }

            _epoch = value;
        }
    }
    public int DatacenterIdBits
        {
            get => _datacenterIdBits;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Datacenter ID bits can't be less than zero.");
                }

                _datacenterIdBits = value;
            }
        }

        public int WorkerIdBits
        {
            get => _workerIdBits;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value),
                        value,
                        "Worker ID bits can't be less than zero.");
                }

                _workerIdBits = value;
            }
        }

    public SnowFlakeModel(
        IDateTimeProvider dateTimeProvider,
        DateTime epoch,
        int datacenterIdBits,
        int workerIdBits) 
    {
        _dateTimeProvider = dateTimeProvider;
        Epoch = epoch;
        DatacenterIdBits = datacenterIdBits;
        WorkerIdBits = workerIdBits;
    }

    public SnowFlakeModel(
        IDateTimeProvider dateTimeProvider,
        DateTime epoch) 
    {
        _dateTimeProvider = dateTimeProvider;
        Epoch = epoch;
    }
}