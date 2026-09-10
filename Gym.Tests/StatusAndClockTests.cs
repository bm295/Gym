using Gym.Application;
using Gym.Domain;
using Gym.Infrastructure;

namespace Gym.Tests;

public sealed class StatusAndClockTests
{
    [Fact]
    public void Member_and_subscription_statuses_are_strongly_typed()
    {
        Assert.Equal(
            [MemberStatus.Draft, MemberStatus.Active, MemberStatus.Inactive, MemberStatus.Suspended],
            Enum.GetValues<MemberStatus>());
        Assert.Equal(
            [
                SubscriptionStatus.Pending,
                SubscriptionStatus.Active,
                SubscriptionStatus.Expired,
                SubscriptionStatus.Suspended,
                SubscriptionStatus.Cancelled,
                SubscriptionStatus.Voided
            ],
            Enum.GetValues<SubscriptionStatus>());
        Assert.Equal(typeof(MemberStatus), typeof(Member).GetProperty(nameof(Member.Status))!.PropertyType);
        Assert.Equal(typeof(SubscriptionStatus), typeof(Subscription).GetProperty(nameof(Subscription.Status))!.PropertyType);
    }

    [Fact]
    public void Utc_clock_returns_a_utc_timestamp()
    {
        IUtcClock clock = new UtcClock();

        Assert.Equal(TimeSpan.Zero, clock.UtcNow.Offset);
    }
}
