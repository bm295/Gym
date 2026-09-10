using Gym.Domain;

namespace Gym.Application.Repositories;

public interface IMemberRepository
{
    Member? FindByCodeOrNormalizedPhone(Guid tenantId, string memberCodeOrNormalizedPhone);

    void Add(Member member);
}
