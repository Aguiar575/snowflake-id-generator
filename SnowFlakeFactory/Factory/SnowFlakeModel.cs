using System.ComponentModel.DataAnnotations;
using SnowFlakeFactory.Interface;
using SnowFlakeFactory.Service;

namespace SnowFlakeFactory.Model;

public class SnowFlakeModel
{
    private int _datacenterIdBits = 4;
    private int _workerIdBits = 6;
    private DateTime _epoch;
    private readonly IDateTimeProvider _dateTimeProvider;
    private ulong _maxDatacenterId => (ulong)(-1L ^ (-1L << this.DatacenterIdBits));
    private ulong _maxWorkerId => (ulong)(-1L ^ (-1L << this.WorkerIdBits));

    public ulong WorkerId { get; init; } = 0UL;
    public ulong DatacenterId { get; init; } = 0UL;

    public DateTime Epoch 
    { 
        get => _epoch; 
        set {
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

    public SnowFlakeModel() =>
        _dateTimeProvider = new DateTimeProvider();

    public SnowFlakeModel(IDateTimeProvider dateTimeProvider) =>
        _dateTimeProvider = dateTimeProvider;

    public void Validate()
    {
        if (this.DatacenterId > _maxDatacenterId)
            throw new ValidationException(
                $"Datacenter ID '{this.DatacenterId}' can't be greater than {_maxDatacenterId}.");
                
        if (this.WorkerId > _maxWorkerId)
            throw new ValidationException(
                $"Worker ID '{this.WorkerId}' can't be greater than {_maxWorkerId}.");
    }
}