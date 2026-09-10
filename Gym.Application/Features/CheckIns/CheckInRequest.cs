namespace Gym.Application.CheckIns;

public sealed record CheckInRequest(
    Guid TenantId,
    Guid BranchId,
    string MemberCodeOrNormalizedPhone,
    Guid? StaffUserId = null,
    string? Note = null);
