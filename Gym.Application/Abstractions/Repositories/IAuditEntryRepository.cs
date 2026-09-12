using Gym.Domain;

namespace Gym.Application.Repositories;

public interface IAuditEntryRepository
{
    IReadOnlyList<AuditEntry> List(Guid tenantId);
    void Add(AuditEntry entry);
}
