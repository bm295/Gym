using Gym.Application.CheckIns.Contracts;

namespace Gym.WebUI.Services.CheckIns;

public interface ICheckInApiClient
{
    Task<CheckInApiResponse> CheckInAsync(
        Guid tenantId,
        Guid branchId,
        CheckInApiRequest request,
        CancellationToken cancellationToken = default);
}
