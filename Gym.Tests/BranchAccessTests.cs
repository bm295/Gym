using Gym.Application.Contracts.Tenants;
using Gym.Application.Tenants.Access;

namespace Gym.Tests;

public sealed class BranchAccessTests
{
    [Fact]
    public void Tenant_admin_has_tenant_wide_branch_access()
    {
        var access = BranchAccess.TenantAdmin();

        Assert.Equal(TenantStaffRole.TenantAdmin, access.Role);
        Assert.Equal(BranchAccessScope.TenantWide, access.Scope);
        Assert.True(access.CanAccess(Guid.NewGuid()));
    }

    [Fact]
    public void Branch_manager_and_receptionist_have_only_explicit_branch_access()
    {
        var allowedBranch = Guid.NewGuid();
        var manager = BranchAccess.BranchManager([allowedBranch]);
        var receptionist = BranchAccess.Receptionist([allowedBranch]);

        Assert.True(manager.CanAccess(allowedBranch));
        Assert.True(receptionist.CanAccess(allowedBranch));
        Assert.False(manager.CanAccess(Guid.NewGuid()));
        Assert.False(receptionist.CanAccess(Guid.NewGuid()));
    }

    [Fact]
    public void Explicit_access_requires_at_least_one_branch()
    {
        Assert.Throws<ArgumentException>(() => BranchAccess.Receptionist([]));
    }
}
