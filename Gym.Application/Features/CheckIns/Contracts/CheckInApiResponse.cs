using Gym.Application.CheckIns;

namespace Gym.Application.CheckIns.Contracts;

public sealed record CheckInApiResponse(
    bool IsSuccess,
    Guid? CheckInId,
    CheckInRejectionReason? RejectionReason);
