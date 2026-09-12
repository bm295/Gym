namespace Gym.Domain;

/// <summary>
/// Grants a staff user access to one branch within their tenant.
/// Tenant administrators use tenant-wide access and do not require assignments.
/// </summary>
public sealed class StaffBranchAccessAssignment
{
    public required Guid TenantId { get; init; }
    public required Guid StaffUserId { get; init; }
    public required Guid BranchId { get; init; }
}
