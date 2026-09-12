using Gym.Application.Contracts.Tenants;

namespace Gym.Tests;

public sealed class TenantContextContractTests
{
    [Fact]
    public void Carries_the_authenticated_tenant_staff_role_and_allowed_branches()
    {
        var tenantId = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var branch = new AllowedBranchContext(Guid.NewGuid(), "HCM-01", "District 1");

        var context = new TenantContext(
            tenantId,
            "GymOS Fitness",
            new AuthenticatedStaffContext(staffId, "Linh Nguyen", TenantStaffRole.Receptionist),
            [branch]);

        Assert.Equal(tenantId, context.TenantId);
        Assert.Equal(staffId, context.Staff.StaffUserId);
        Assert.Equal(TenantStaffRole.Receptionist, context.Staff.Role);
        Assert.Single(context.AllowedBranches);
        Assert.Equal(branch, context.AllowedBranches[0]);
    }
}
