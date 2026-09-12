using Gym.Domain;

namespace Gym.Application.Repositories;

public interface ISubscriptionRepository
{
    Subscription? GetById(Guid tenantId, Guid subscriptionId);
    IReadOnlyList<Subscription> List(Guid tenantId);
    IReadOnlyList<Subscription> GetByMember(Guid tenantId, Guid memberId);

    void Add(Subscription subscription);
}
