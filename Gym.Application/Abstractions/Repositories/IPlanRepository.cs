using Gym.Domain;

namespace Gym.Application.Repositories;

public interface IPlanRepository
{
    MembershipPlan? GetById(Guid tenantId, Guid planId);
    IReadOnlyList<MembershipPlan> List(Guid tenantId);
    void Add(MembershipPlan plan);
}
