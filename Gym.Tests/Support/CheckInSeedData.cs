using Gym.Domain;

namespace Gym.Tests.Support;

internal static class CheckInSeedData
{
    public static readonly DateTimeOffset Timestamp = new(2026, 9, 10, 8, 0, 0, TimeSpan.Zero);

    public static Branch ActiveBranch(Guid tenantId, Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        TenantId = tenantId,
        BranchCode = "HCM-01",
        Name = "District 1",
        Status = "Active",
        CreatedAt = Timestamp,
        UpdatedAt = Timestamp
    };

    public static Member ActiveMember(Guid tenantId, Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        TenantId = tenantId,
        MemberCode = "MEM-001",
        FullName = "Test Member",
        PhoneNumber = "0900000000",
        NormalizedPhoneNumber = "0900000000",
        Status = MemberStatus.Active,
        CreatedAt = Timestamp,
        UpdatedAt = Timestamp
    };

    public static Subscription ActiveSubscription(Guid tenantId, Guid memberId, Guid branchId, Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        TenantId = tenantId,
        MemberId = memberId,
        MemberPlanId = Guid.NewGuid(),
        HomeBranchId = branchId,
        Status = SubscriptionStatus.Active,
        StartDate = DateOnly.FromDateTime(Timestamp.UtcDateTime).AddDays(-1),
        EndDate = DateOnly.FromDateTime(Timestamp.UtcDateTime).AddDays(30),
        SalePrice = 500_000m,
        AmountPaid = 500_000m,
        Currency = "VND",
        AllowCrossBranchSnapshot = false,
        PlanNameSnapshot = "Monthly",
        DurationDaysSnapshot = 31,
        OriginalPlanPriceSnapshot = 500_000m,
        CreatedAt = Timestamp,
        UpdatedAt = Timestamp
    };

    public static CheckIn CheckIn(Guid tenantId, Guid memberId, Guid subscriptionId, Guid branchId, DateTimeOffset? checkedInAt = null) => new()
    {
        Id = Guid.NewGuid(),
        TenantId = tenantId,
        MemberId = memberId,
        SubscriptionId = subscriptionId,
        BranchId = branchId,
        CheckedInAt = checkedInAt ?? Timestamp
    };
}
