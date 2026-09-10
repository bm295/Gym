using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryCheckInRepository(InMemoryGymDataStore store) : ICheckInRepository
{
    public IReadOnlyList<CheckIn> GetByMemberAndBranchSince(
        Guid tenantId,
        Guid memberId,
        Guid branchId,
        DateTimeOffset since) =>
        store.GetCheckInsByMemberAndBranchSince(tenantId, memberId, branchId, since);

    public void Add(CheckIn checkIn) => store.Add(checkIn);
}
