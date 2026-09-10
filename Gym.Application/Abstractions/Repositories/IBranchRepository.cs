using Gym.Domain;

namespace Gym.Application.Repositories;

public interface IBranchRepository
{
    Branch? GetById(Guid tenantId, Guid branchId);

    void Add(Branch branch);
}
