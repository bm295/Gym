using Gym.Application.Contracts.Reports;

namespace Gym.Application.Repositories;

public interface ITenantReportQuery
{
    TenantOperationalReport GetOperationalReport(Guid tenantId);
}
