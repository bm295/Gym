using Gym.Application;
using Gym.Application.CheckIns;
using Gym.Domain;
using Gym.Infrastructure;
using Gym.Infrastructure.Repositories;
using Gym.Tests.Support;

namespace Gym.Tests;

public sealed class CheckInServiceValidationTests
{
    [Theory]
    [InlineData(MemberStatus.Inactive)]
    [InlineData(MemberStatus.Suspended)]
    public void Rejects_a_member_who_is_not_active(MemberStatus status)
    {
        var context = CreateContext(memberStatus: status);

        AssertRejected(context, CheckInRejectionReason.MemberInactive);
    }

    [Theory]
    [InlineData(SubscriptionStatus.Pending)]
    [InlineData(SubscriptionStatus.Expired)]
    [InlineData(SubscriptionStatus.Suspended)]
    [InlineData(SubscriptionStatus.Cancelled)]
    [InlineData(SubscriptionStatus.Voided)]
    public void Rejects_when_there_is_no_active_subscription(SubscriptionStatus status)
    {
        var context = CreateContext(subscriptionStatus: status);

        AssertRejected(context, CheckInRejectionReason.NoActiveSubscription);
    }

    [Fact]
    public void Rejects_an_active_subscription_that_has_not_started()
    {
        var context = CreateContext(startDate: CurrentDate.AddDays(1));

        AssertRejected(context, CheckInRejectionReason.SubscriptionNotStarted);
    }

    [Fact]
    public void Rejects_an_active_subscription_that_has_expired()
    {
        var context = CreateContext(endDate: CurrentDate.AddDays(-1));

        AssertRejected(context, CheckInRejectionReason.SubscriptionExpired);
    }

    [Fact]
    public void Accepts_subscription_start_and_end_dates_inclusively()
    {
        var context = CreateContext(startDate: CurrentDate, endDate: CurrentDate);

        var result = context.Service.CheckIn(context.Request);

        Assert.True(result.IsSuccess);
        Assert.Single(context.Store.GetCheckInsByMemberAndBranchSince(
            context.Request.TenantId, context.Member.Id, context.Request.BranchId, DateTimeOffset.MinValue));
    }

    [Fact]
    public void Rejects_a_branch_outside_the_tenant_without_disclosing_it()
    {
        var context = CreateContext();
        var foreignBranch = CheckInSeedData.ActiveBranch(Guid.NewGuid());
        context.Store.Add(foreignBranch);
        var request = context.Request with { BranchId = foreignBranch.Id };

        var result = context.Service.CheckIn(request);

        Assert.Equal(CheckInRejectionReason.BranchNotPermitted, result.RejectionReason);
        AssertNoCheckIns(context);
    }

    [Fact]
    public void Rejects_a_different_home_branch_when_cross_branch_access_is_disabled()
    {
        var context = CreateContext();
        var otherBranch = CheckInSeedData.ActiveBranch(context.Request.TenantId);
        context.Store.Add(otherBranch);
        var request = context.Request with { BranchId = otherBranch.Id };

        var result = context.Service.CheckIn(request);

        Assert.Equal(CheckInRejectionReason.BranchNotPermitted, result.RejectionReason);
        AssertNoCheckIns(context);
    }

    [Fact]
    public void Rejects_a_recent_check_in_at_the_same_branch()
    {
        var context = CreateContext();
        context.Store.Add(CheckInSeedData.CheckIn(
            context.Request.TenantId,
            context.Member.Id,
            context.Subscription.Id,
            context.Request.BranchId,
            CheckInSeedData.Timestamp.AddMinutes(-4)));

        var result = context.Service.CheckIn(context.Request);

        Assert.Equal(CheckInRejectionReason.DuplicateCheckIn, result.RejectionReason);
        Assert.Single(context.Store.GetCheckInsByMemberAndBranchSince(
            context.Request.TenantId, context.Member.Id, context.Request.BranchId, DateTimeOffset.MinValue));
    }

    [Fact]
    public void Allows_a_check_in_after_the_duplicate_window()
    {
        var context = CreateContext();
        context.Store.Add(CheckInSeedData.CheckIn(
            context.Request.TenantId,
            context.Member.Id,
            context.Subscription.Id,
            context.Request.BranchId,
            CheckInSeedData.Timestamp.AddMinutes(-6)));

        var result = context.Service.CheckIn(context.Request);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, context.Store.GetCheckInsByMemberAndBranchSince(
            context.Request.TenantId, context.Member.Id, context.Request.BranchId, DateTimeOffset.MinValue).Count);
    }

    private static readonly DateOnly CurrentDate =
        DateOnly.FromDateTime(CheckInSeedData.Timestamp.UtcDateTime);

    private static TestContext CreateContext(
        MemberStatus memberStatus = MemberStatus.Active,
        SubscriptionStatus subscriptionStatus = SubscriptionStatus.Active,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        var seededMember = CheckInSeedData.ActiveMember(tenantId);
        var member = CopyMemberWithStatus(seededMember, memberStatus);
        var seededSubscription = CheckInSeedData.ActiveSubscription(tenantId, member.Id, branch.Id);
        var subscription = CopySubscription(
            seededSubscription,
            subscriptionStatus,
            startDate ?? seededSubscription.StartDate,
            endDate ?? seededSubscription.EndDate);
        var store = new InMemoryGymDataStore();
        store.Add(branch);
        store.Add(member);
        store.Add(subscription);

        var service = new CheckInService(
            new InMemoryMemberRepository(store),
            new InMemoryBranchRepository(store),
            new InMemorySubscriptionRepository(store),
            new InMemoryCheckInRepository(store),
            new FakeClock(CheckInSeedData.Timestamp),
            new NoOpCheckInOperationLogger());
        var request = new CheckInRequest(tenantId, branch.Id, member.MemberCode);
        return new TestContext(service, store, member, subscription, request);
    }

    private static void AssertRejected(TestContext context, CheckInRejectionReason reason)
    {
        var result = context.Service.CheckIn(context.Request);

        Assert.False(result.IsSuccess);
        Assert.Equal(reason, result.RejectionReason);
        AssertNoCheckIns(context);
    }

    private static void AssertNoCheckIns(TestContext context) =>
        Assert.Empty(context.Store.GetCheckInsByMemberAndBranchSince(
            context.Request.TenantId, context.Member.Id, context.Request.BranchId, DateTimeOffset.MinValue));

    private static Member CopyMemberWithStatus(Member member, MemberStatus status) => new()
    {
        Id = member.Id,
        TenantId = member.TenantId,
        MemberCode = member.MemberCode,
        FullName = member.FullName,
        PhoneNumber = member.PhoneNumber,
        NormalizedPhoneNumber = member.NormalizedPhoneNumber,
        Status = status,
        CreatedAt = member.CreatedAt,
        UpdatedAt = member.UpdatedAt
    };

    private static Subscription CopySubscription(
        Subscription subscription,
        SubscriptionStatus status,
        DateOnly startDate,
        DateOnly endDate) => new()
    {
        Id = subscription.Id,
        TenantId = subscription.TenantId,
        MemberId = subscription.MemberId,
        MemberPlanId = subscription.MemberPlanId,
        HomeBranchId = subscription.HomeBranchId,
        Status = status,
        StartDate = startDate,
        EndDate = endDate,
        SalePrice = subscription.SalePrice,
        AmountPaid = subscription.AmountPaid,
        Currency = subscription.Currency,
        RemainingVisits = subscription.RemainingVisits,
        AllowCrossBranchSnapshot = subscription.AllowCrossBranchSnapshot,
        PlanNameSnapshot = subscription.PlanNameSnapshot,
        DurationDaysSnapshot = subscription.DurationDaysSnapshot,
        VisitLimitSnapshot = subscription.VisitLimitSnapshot,
        OriginalPlanPriceSnapshot = subscription.OriginalPlanPriceSnapshot,
        CreatedAt = subscription.CreatedAt,
        UpdatedAt = subscription.UpdatedAt
    };

    private sealed record TestContext(
        CheckInService Service,
        InMemoryGymDataStore Store,
        Member Member,
        Subscription Subscription,
        CheckInRequest Request);

    private sealed class FakeClock(DateTimeOffset utcNow) : IUtcClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class NoOpCheckInOperationLogger : ICheckInOperationLogger
    {
        public void LogSuccessfulCheckIn(CheckIn checkIn)
        {
        }
    }
}
