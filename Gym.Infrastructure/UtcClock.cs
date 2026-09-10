using Gym.Application;

namespace Gym.Infrastructure;

public sealed class UtcClock : IUtcClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
