namespace Gym.Domain;

public sealed class Tenant : AuditableEntity
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Status { get; init; }
    public required string Timezone { get; init; }
}
