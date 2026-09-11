using Gym.Application;
using Gym.Application.CheckIns;
using Gym.Infrastructure;
using Gym.Infrastructure.Repositories;
using Gym.Tests.Support;

namespace Gym.Tests;

public sealed class CheckInServiceLookupTests
{
    [Theory]
    [InlineData("mem-001")]
    [InlineData("0900000000")]
    [InlineData("  MEM-001  ")]
    public void Check_in_finds_a_member_by_code_or_normalized_phone(string lookup)
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        var member = CheckInSeedData.ActiveMember(tenantId);
        var subscription = CheckInSeedData.ActiveSubscription(tenantId, member.Id, branch.Id);
        var (service, store) = CreateService();
        store.Add(branch);
        store.Add(member);
        store.Add(subscription);

        var result = service.CheckIn(new CheckInRequest(tenantId, branch.Id, lookup));

        Assert.True(result.IsSuccess);
        Assert.Single(store.GetCheckInsByMemberAndBranchSince(
            tenantId, member.Id, branch.Id, DateTimeOffset.MinValue));
    }

    [Theory]
    [InlineData("unknown")]
    [InlineData("")]
    [InlineData("   ")]
    public void Check_in_rejects_an_unknown_or_empty_lookup(string lookup)
    {
        var tenantId = Guid.NewGuid();
        var (service, store) = CreateService();
        store.Add(CheckInSeedData.ActiveMember(tenantId));

        var result = service.CheckIn(new CheckInRequest(tenantId, Guid.NewGuid(), lookup));

        Assert.False(result.IsSuccess);
        Assert.Equal(CheckInRejectionReason.MemberNotFound, result.RejectionReason);
    }

    [Fact]
    public void Check_in_does_not_find_a_member_from_another_tenant()
    {
        var memberTenantId = Guid.NewGuid();
        var (service, store) = CreateService();
        var member = CheckInSeedData.ActiveMember(memberTenantId);
        store.Add(member);

        var result = service.CheckIn(new CheckInRequest(
            Guid.NewGuid(), Guid.NewGuid(), member.MemberCode));

        Assert.False(result.IsSuccess);
        Assert.Equal(CheckInRejectionReason.MemberNotFound, result.RejectionReason);
    }

    private static (CheckInService Service, InMemoryGymDataStore Store) CreateService()
    {
        var store = new InMemoryGymDataStore();
        var service = new CheckInService(
            new InMemoryMemberRepository(store),
            new InMemoryBranchRepository(store),
            new InMemorySubscriptionRepository(store),
            new InMemoryCheckInRepository(store),
            new FakeClock(CheckInSeedData.Timestamp),
            new NoOpCheckInOperationLogger());
        return (service, store);
    }

    private sealed class FakeClock(DateTimeOffset utcNow) : IUtcClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class NoOpCheckInOperationLogger : ICheckInOperationLogger
    {
        public void LogSuccessfulCheckIn(Gym.Domain.CheckIn checkIn)
        {
        }
    }
}
