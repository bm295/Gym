using Gym.Application.Repositories;
using Gym.Infrastructure.Repositories;
using Gym.Infrastructure.SeedData;

namespace Gym.Tests;

public sealed class TenantScopedRepositoryTests
{
    private readonly InMemoryGymDataStore _store = new();

    [Fact]
    public void Member_plan_subscription_payment_checkin_and_audit_queries_do_not_cross_tenant_boundaries()
    {
        var atlasTenantId = InMemoryGymDataSeeder.AtlasFitnessTenantId;
        var pulseTenantId = InMemoryGymDataSeeder.PulseWellnessTenantId;
        IMemberRepository members = new InMemoryMemberRepository(_store);
        IPlanRepository plans = new InMemoryPlanRepository(_store);
        ISubscriptionRepository subscriptions = new InMemorySubscriptionRepository(_store);
        IPaymentRepository payments = new InMemoryPaymentRepository(_store);
        ICheckInRepository checkIns = new InMemoryCheckInRepository(_store);
        IAuditEntryRepository auditEntries = new InMemoryAuditEntryRepository(_store);

        var atlasMember = Assert.Single(members.List(atlasTenantId));
        var atlasPlan = Assert.Single(plans.List(atlasTenantId));
        var atlasSubscription = Assert.Single(subscriptions.List(atlasTenantId));
        var atlasPayment = Assert.Single(payments.List(atlasTenantId));
        var atlasCheckIn = Assert.Single(checkIns.List(atlasTenantId));
        var atlasAuditEntry = Assert.Single(auditEntries.List(atlasTenantId));

        Assert.Null(members.GetById(pulseTenantId, atlasMember.Id));
        Assert.Null(plans.GetById(pulseTenantId, atlasPlan.Id));
        Assert.Null(subscriptions.GetById(pulseTenantId, atlasSubscription.Id));
        Assert.Null(payments.GetById(pulseTenantId, atlasPayment.Id));
        Assert.Null(checkIns.GetById(pulseTenantId, atlasCheckIn.Id));
        Assert.DoesNotContain(auditEntries.List(pulseTenantId), entry => entry.Id == atlasAuditEntry.Id);
    }

    [Fact]
    public void Operational_report_is_calculated_only_for_the_requested_tenant()
    {
        ITenantReportQuery reports = new InMemoryTenantReportQuery(_store);

        var atlas = reports.GetOperationalReport(InMemoryGymDataSeeder.AtlasFitnessTenantId);
        var pulse = reports.GetOperationalReport(InMemoryGymDataSeeder.PulseWellnessTenantId);

        Assert.Equal(InMemoryGymDataSeeder.AtlasFitnessTenantId, atlas.TenantId);
        Assert.Equal(1, atlas.MemberCount);
        Assert.Equal(1, atlas.ActiveSubscriptionCount);
        Assert.Equal(1, atlas.PaymentCount);
        Assert.Equal(1, atlas.CheckInCount);
        Assert.Equal(InMemoryGymDataSeeder.PulseWellnessTenantId, pulse.TenantId);
        Assert.Equal(0, pulse.CheckInCount);
    }
}
