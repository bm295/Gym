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

    [Theory]
    [InlineData(3, 2)]
    [InlineData(1, 0)]
    public void Consumes_exactly_one_visit_and_persists_one_check_in(
        int remainingVisits, int expectedRemainingVisits)
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        var member = CheckInSeedData.ActiveMember(tenantId);
        var subscription = CheckInSeedData.ActiveSubscription(
            tenantId, member.Id, branch.Id, remainingVisits: remainingVisits);
        var store = new InMemoryGymDataStore();
        store.Add(branch);
        store.Add(member);
        store.Add(subscription);
        var subscriptions = new InMemorySubscriptionRepository(store);
        var logger = new RecordingCheckInOperationLogger();
        var service = new CheckInService(
            new InMemoryMemberRepository(store),
            new InMemoryBranchRepository(store),
            subscriptions,
            new InMemoryCheckInRepository(store),
            new FakeClock(CheckInSeedData.Timestamp),
            logger);
        var request = new CheckInRequest(tenantId, branch.Id, member.MemberCode);

        var result = service.CheckIn(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedRemainingVisits, subscriptions.GetById(tenantId, subscription.Id)!.RemainingVisits);
        Assert.Equal(remainingVisits, subscription.VisitLimitSnapshot);
        var checkIn = Assert.Single(store.GetCheckInsByMemberAndBranchSince(
            tenantId, member.Id, branch.Id, DateTimeOffset.MinValue));
        Assert.Equal(result.CheckInId, checkIn.Id);
        Assert.Equal(subscription.Id, checkIn.SubscriptionId);
        Assert.Same(checkIn, Assert.Single(logger.SuccessfulCheckIns));

        var retry = service.CheckIn(request);

        Assert.False(retry.IsSuccess);
        Assert.Equal(expectedRemainingVisits, subscription.RemainingVisits);
        Assert.Single(store.GetCheckInsByMemberAndBranchSince(
            tenantId, member.Id, branch.Id, DateTimeOffset.MinValue));
        Assert.Single(logger.SuccessfulCheckIns);
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
