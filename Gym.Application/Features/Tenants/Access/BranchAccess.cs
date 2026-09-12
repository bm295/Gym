using Gym.Application.Contracts.Tenants;

namespace Gym.Application.Tenants.Access;

/// <summary>
/// Application policy that determines the branches a staff member may access.
/// </summary>
public sealed record BranchAccess(
    TenantStaffRole Role,
    BranchAccessScope Scope,
    IReadOnlySet<Guid> ExplicitBranchIds)
{
    public static BranchAccess TenantAdmin() => new(
        TenantStaffRole.TenantAdmin,
        BranchAccessScope.TenantWide,
        new HashSet<Guid>());

    public static BranchAccess BranchManager(IEnumerable<Guid> branchIds) =>
        ForExplicitBranches(TenantStaffRole.BranchManager, branchIds);

    public static BranchAccess Receptionist(IEnumerable<Guid> branchIds) =>
        ForExplicitBranches(TenantStaffRole.Receptionist, branchIds);

    public bool CanAccess(Guid branchId) =>
        Scope == BranchAccessScope.TenantWide || ExplicitBranchIds.Contains(branchId);

    private static BranchAccess ForExplicitBranches(TenantStaffRole role, IEnumerable<Guid> branchIds)
    {
        var branches = new HashSet<Guid>(branchIds);
        if (branches.Count == 0)
        {
            throw new ArgumentException("Explicit branch access requires at least one branch.", nameof(branchIds));
        }

        return new BranchAccess(role, BranchAccessScope.ExplicitBranches, branches);
    }
}
