namespace Gym.Application.CheckIns.Contracts;

public sealed record CheckInApiRequest(string MemberCodeOrNormalizedPhone, string? Note = null);
