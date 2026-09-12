using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryMemberRepository(InMemoryGymDataStore store) : IMemberRepository
{
    public Member? GetById(Guid tenantId, Guid memberId) =>
        store.GetMembers(tenantId).SingleOrDefault(member => member.Id == memberId);

    public IReadOnlyList<Member> List(Guid tenantId) => store.GetMembers(tenantId);

    public Member? FindByCodeOrNormalizedPhone(Guid tenantId, string memberCodeOrNormalizedPhone) =>
        store.FindMemberByCodeOrNormalizedPhone(tenantId, memberCodeOrNormalizedPhone);

    public void Add(Member member) => store.Add(member);
}
