using Gym.Application.Contracts.Reports;

namespace Gym.Application.Repositories;

public interface ITenantReportQuery
{
    TenantOperationalReport GetOperationalReport(Guid tenantId);
    AttendanceReport GetAttendanceReport(Guid tenantId, Guid branchId, DateOnly startDate, DateOnly endDate, TimeZoneInfo timeZone);
}
