namespace Gym.Domain;

public sealed class Branch : SoftDeletableEntity
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string BranchCode { get; init; }
    public required string Name { get; init; }
    public string? Address { get; init; }
    public required string Status { get; init; }
}
