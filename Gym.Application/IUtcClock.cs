namespace Gym.Application;

public interface IUtcClock
{
    DateTimeOffset UtcNow { get; }
}
