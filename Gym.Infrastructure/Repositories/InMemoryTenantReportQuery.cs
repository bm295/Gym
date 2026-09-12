using Gym.Application.Contracts.Reports;
using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryTenantReportQuery(InMemoryGymDataStore store) : ITenantReportQuery
{
    public TenantOperationalReport GetOperationalReport(Guid tenantId) => new(
        tenantId,
        store.GetMembers(tenantId).Count,
        store.GetSubscriptions(tenantId).Count(subscription => subscription.Status == SubscriptionStatus.Active),
        store.GetPayments(tenantId).Count,
        store.GetCheckIns(tenantId).Count);
}
