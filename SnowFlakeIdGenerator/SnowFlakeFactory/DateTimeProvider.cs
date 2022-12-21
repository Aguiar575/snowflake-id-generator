using SnowFlakeFactory.Interface;

namespace SnowFlakeFactory.Service;

public class DateTimeProvider : IDateTimeProvider 
{
    public DateTime GetUtcNow() =>
        DateTime.UtcNow;

    public DateTime GetToday() => 
        DateTime.Today;
}