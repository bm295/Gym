namespace Gym.Application.Contracts.Reports;

/// <summary>Tenant-scoped operational summary used by dashboard/report queries.</summary>
public sealed record TenantOperationalReport(
    Guid TenantId,
    int MemberCount,
    int ActiveSubscriptionCount,
    int PaymentCount,
    int CheckInCount);
