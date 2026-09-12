namespace Gym.Application.Contracts.Tenants;

/// <summary>
/// Claim names issued by the identity provider for an authenticated tenant staff member.
/// </summary>
public static class TenantClaimTypes
{
    public const string TenantId = "tenant_id";
    public const string TenantName = "tenant_name";
    public const string BranchContext = "branch_context";
}
