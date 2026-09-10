using Gym.Domain;

namespace Gym.Application.Repositories;

public interface ICheckInRepository
{
    IReadOnlyList<CheckIn> GetByMemberAndBranchSince(
        Guid tenantId,
        Guid memberId,
        Guid branchId,
        DateTimeOffset since);

    void Add(CheckIn checkIn);
}
