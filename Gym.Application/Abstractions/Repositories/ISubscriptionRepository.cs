using Gym.Domain;

namespace Gym.Application.Repositories;

public interface ISubscriptionRepository
{
    IReadOnlyList<Subscription> GetByMember(Guid tenantId, Guid memberId);

    void Add(Subscription subscription);
}
