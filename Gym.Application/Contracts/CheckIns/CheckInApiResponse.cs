using Gym.Application.CheckIns;

namespace Gym.Application.Contracts.CheckIns;

public sealed record CheckInApiResponse(
    bool IsSuccess,
    Guid? CheckInId,
    CheckInRejectionReason? RejectionReason);
