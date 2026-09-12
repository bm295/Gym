using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryPlanRepository(InMemoryGymDataStore store) : IPlanRepository
{
    public MembershipPlan? GetById(Guid tenantId, Guid planId) =>
        store.GetMembershipPlans(tenantId).SingleOrDefault(plan => plan.Id == planId);

    public IReadOnlyList<MembershipPlan> List(Guid tenantId) => store.GetMembershipPlans(tenantId);

    public void Add(MembershipPlan plan) => store.Add(plan);
}
