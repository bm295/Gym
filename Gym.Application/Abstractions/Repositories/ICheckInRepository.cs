using Gym.Domain;

namespace Gym.Application.Repositories;

public interface ICheckInRepository
{
    CheckIn? GetById(Guid tenantId, Guid checkInId);
    IReadOnlyList<CheckIn> List(Guid tenantId);
    IReadOnlyList<CheckIn> GetByMemberAndBranchSince(
        Guid tenantId,
        Guid memberId,
        Guid branchId,
        DateTimeOffset since);

    void Add(CheckIn checkIn);
}
