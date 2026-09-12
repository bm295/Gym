using Gym.Infrastructure.Repositories;
using Gym.Infrastructure.SeedData;

namespace Gym.Tests;

public sealed class InMemoryGymDataSeederTests
{
    [Fact]
    public void Seeds_two_isolated_tenants_with_operational_records()
    {
        var store = new InMemoryGymDataStore();

        Assert.Equal(2, store.GetTenants().Count);
        Assert.Equal(2, store.GetBranches(InMemoryGymDataSeeder.AtlasFitnessTenantId).Count);
        Assert.Single(store.GetBranches(InMemoryGymDataSeeder.PulseWellnessTenantId));
        Assert.Single(store.GetMembers(InMemoryGymDataSeeder.AtlasFitnessTenantId));
        Assert.Single(store.GetMembershipPlans(InMemoryGymDataSeeder.AtlasFitnessTenantId));
        Assert.Single(store.GetSubscriptionsByMember(
            InMemoryGymDataSeeder.AtlasFitnessTenantId,
            store.GetMembers(InMemoryGymDataSeeder.AtlasFitnessTenantId).Single().Id));
        Assert.Single(store.GetPayments(InMemoryGymDataSeeder.AtlasFitnessTenantId));
        Assert.Single(store.GetCheckIns(InMemoryGymDataSeeder.AtlasFitnessTenantId));
        Assert.Empty(store.GetMembers(Guid.NewGuid()));
        Assert.Empty(store.GetPayments(Guid.NewGuid()));
    }

    [Fact]
    public void Seeds_explicit_branch_access_for_non_tenant_admin_staff()
    {
        var store = new InMemoryGymDataStore();
        var receptionist = store.GetStaffUsers(InMemoryGymDataSeeder.AtlasFitnessTenantId)
            .Single(staff => staff.Role == "Receptionist");

        var assignments = store.GetBranchAccessAssignments(
            InMemoryGymDataSeeder.AtlasFitnessTenantId,
            receptionist.Id);

        Assert.Equal(InMemoryGymDataSeeder.AtlasDistrictOneBranchId, Assert.Single(assignments).BranchId);
        var tenantAdmin = store.GetStaffUsers(InMemoryGymDataSeeder.AtlasFitnessTenantId)
            .Single(staff => staff.Role == "TenantAdmin");
        Assert.Empty(store.GetBranchAccessAssignments(InMemoryGymDataSeeder.AtlasFitnessTenantId, tenantAdmin.Id));
    }
}
