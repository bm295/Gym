using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryBranchRepository(InMemoryGymDataStore store) : IBranchRepository
{
    public Branch? GetById(Guid tenantId, Guid branchId) => store.GetBranchById(tenantId, branchId);

    public void Add(Branch branch) => store.Add(branch);
}
