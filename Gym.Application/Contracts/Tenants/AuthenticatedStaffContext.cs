namespace Gym.Application.Contracts.Tenants;

public sealed record AuthenticatedStaffContext(
    Guid StaffUserId,
    string DisplayName,
    TenantStaffRole Role);
