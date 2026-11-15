using Gym.Application.Contracts.Reports;
using Gym.Application.Repositories;
using Gym.Domain;
using Gym.Application.Reports;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryTenantReportQuery(InMemoryGymDataStore store) : ITenantReportQuery
{
    public AttendanceReport GetAttendanceReport(Guid tenantId, Guid branchId, DateOnly startDate, DateOnly endDate, TimeZoneInfo timeZone) =>
        AttendanceReportCalculator.Calculate(
            store.GetCheckIns(tenantId).Where(item => item.BranchId == branchId).Select(item => item.CheckedInAt),
            startDate, endDate, timeZone);

    public MembershipUtilizationReport GetMembershipUtilizationReport(Guid tenantId, Guid branchId, DateOnly startDate, DateOnly endDate) =>
        MembershipAnalyticsCalculator.Calculate(startDate, endDate,
            store.GetSubscriptions(tenantId)
                .Where(subscription => subscription.HomeBranchId == branchId
                    && subscription.Status == SubscriptionStatus.Active)
                .Select(subscription => (subscription.StartDate, subscription.EndDate)));

    public TenantOperationalReport GetOperationalReport(Guid tenantId) => new(
        tenantId,
        store.GetMembers(tenantId).Count,
        store.GetSubscriptions(tenantId).Count(subscription => subscription.Status == SubscriptionStatus.Active),
        store.GetPayments(tenantId).Count,
        store.GetCheckIns(tenantId).Count);
}
