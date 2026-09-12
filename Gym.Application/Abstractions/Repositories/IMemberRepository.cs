using Gym.Domain;

namespace Gym.Application.Repositories;

public interface IMemberRepository
{
    Member? GetById(Guid tenantId, Guid memberId);
    IReadOnlyList<Member> List(Guid tenantId);
    Member? FindByCodeOrNormalizedPhone(Guid tenantId, string memberCodeOrNormalizedPhone);

    void Add(Member member);
}
