using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryAuditEntryRepository(InMemoryGymDataStore store) : IAuditEntryRepository
{
    public IReadOnlyList<AuditEntry> List(Guid tenantId) => store.GetAuditEntries(tenantId);

    public void Add(AuditEntry entry) => store.Add(entry);
}
