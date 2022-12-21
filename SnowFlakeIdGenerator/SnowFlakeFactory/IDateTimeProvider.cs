namespace SnowFlakeFactory.Interface;

public interface IDateTimeProvider {
    DateTime GetUtcNow();
    DateTime GetToday();
}