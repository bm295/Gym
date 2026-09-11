using Gym.Application;
using Gym.Application.CheckIns;
using Gym.Domain;
using Gym.Infrastructure;
using Gym.Infrastructure.Repositories;
using Gym.Tests.Support;

namespace Gym.Tests;

public sealed class CheckInServiceSuccessTests
{
    [Fact]
    public void Creates_and_logs_exactly_one_utc_check_in_without_changing_unlimited_visits()
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        var member = CheckInSeedData.ActiveMember(tenantId);
        var subscription = CheckInSeedData.ActiveSubscription(tenantId, member.Id, branch.Id);
        Assert.Null(subscription.RemainingVisits);

        var store = new InMemoryGymDataStore();
        store.Add(branch);
        store.Add(member);
        store.Add(subscription);
        var logger = new RecordingCheckInOperationLogger();
        var localTimestamp = CheckInSeedData.Timestamp.ToOffset(TimeSpan.FromHours(7));
        var service = new CheckInService(
            new InMemoryMemberRepository(store),
            new InMemoryBranchRepository(store),
            new InMemorySubscriptionRepository(store),
            new InMemoryCheckInRepository(store),
            new FakeClock(localTimestamp),
            logger);

        var result = service.CheckIn(new CheckInRequest(
            tenantId,
            branch.Id,
            member.MemberCode,
            Guid.NewGuid(),
            "Front desk"));

        Assert.True(result.IsSuccess);
        var checkIn = Assert.Single(store.GetCheckInsByMemberAndBranchSince(
            tenantId, member.Id, branch.Id, DateTimeOffset.MinValue));
        Assert.Equal(TimeSpan.Zero, checkIn.CheckedInAt.Offset);
        Assert.Equal(CheckInSeedData.Timestamp, checkIn.CheckedInAt);
        Assert.Equal(result.CheckInId, checkIn.Id);
        Assert.Same(checkIn, Assert.Single(logger.SuccessfulCheckIns));
        Assert.Null(subscription.RemainingVisits);
    }

    private sealed class FakeClock(DateTimeOffset utcNow) : IUtcClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class RecordingCheckInOperationLogger : ICheckInOperationLogger
    {
        public List<CheckIn> SuccessfulCheckIns { get; } = [];

        public void LogSuccessfulCheckIn(CheckIn checkIn) => SuccessfulCheckIns.Add(checkIn);
    }
}
