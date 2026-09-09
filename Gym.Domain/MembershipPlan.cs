namespace Gym.Domain;

public sealed class MembershipPlan : SoftDeletableEntity
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string PlanCode { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required int DurationDays { get; init; }
    public int? VisitLimit { get; init; }
    public required decimal DefaultPrice { get; init; }
    public required string Currency { get; init; }
    public required bool AllowCrossBranch { get; init; }
    public required string Status { get; init; }
}
