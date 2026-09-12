namespace Gym.Domain;

public sealed class AuditEntry
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public Guid? BranchId { get; init; }
    public required Guid StaffUserId { get; init; }
    public required string Action { get; init; }
    public required string EntityType { get; init; }
    public required Guid EntityId { get; init; }
    public required DateTimeOffset OccurredAt { get; init; }
    public string? Detail { get; init; }
}
