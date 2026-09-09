namespace Gym.Domain;

public sealed class CheckIn
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid MemberId { get; init; }
    public required Guid SubscriptionId { get; init; }
    public required Guid BranchId { get; init; }
    public required DateTimeOffset CheckedInAt { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? Note { get; init; }
}
