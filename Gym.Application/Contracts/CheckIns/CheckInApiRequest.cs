namespace Gym.Application.Contracts.CheckIns;

public sealed record CheckInApiRequest(string MemberCodeOrNormalizedPhone, string? Note = null);
