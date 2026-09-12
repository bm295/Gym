namespace Gym.Application.Contracts.Tenants;

/// <summary>
/// The authenticated operational scope for one Gym SaaS request or UI session.
/// Tenant identity comes from authentication, never from a front-desk form.
/// </summary>
public sealed record TenantContext(
    Guid TenantId,
    string TenantName,
    AuthenticatedStaffContext Staff,
    IReadOnlyList<AllowedBranchContext> AllowedBranches);
