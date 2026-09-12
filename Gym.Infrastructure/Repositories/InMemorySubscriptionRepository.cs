using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemorySubscriptionRepository(InMemoryGymDataStore store) : ISubscriptionRepository
{
    public Subscription? GetById(Guid tenantId, Guid subscriptionId) =>
        store.GetSubscriptions(tenantId).SingleOrDefault(subscription => subscription.Id == subscriptionId);

    public IReadOnlyList<Subscription> List(Guid tenantId) => store.GetSubscriptions(tenantId);

    public IReadOnlyList<Subscription> GetByMember(Guid tenantId, Guid memberId) =>
        store.GetSubscriptionsByMember(tenantId, memberId);

    public void Add(Subscription subscription) => store.Add(subscription);
}
