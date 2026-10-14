namespace Gym.Domain;

public sealed class Subscription : AuditableEntity
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid MemberId { get; init; }
    public required Guid MemberPlanId { get; init; }
    public required Guid HomeBranchId { get; init; }
    public required SubscriptionStatus Status { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public required decimal SalePrice { get; init; }
    public required decimal AmountPaid { get; init; }
    public required string Currency { get; init; }
    private int? _remainingVisits;

    public int? RemainingVisits
    {
        get => _remainingVisits;
        init => _remainingVisits = value;
    }

    public void ConsumeVisit()
    {
        if (_remainingVisits is null)
        {
            return;
        }

        if (_remainingVisits <= 0)
        {
            throw new InvalidOperationException("No visits remain on this subscription.");
        }

        _remainingVisits--;
    }
    public required bool AllowCrossBranchSnapshot { get; init; }
    public required string PlanNameSnapshot { get; init; }
    public required int DurationDaysSnapshot { get; init; }
    public int? VisitLimitSnapshot { get; init; }
    public required decimal OriginalPlanPriceSnapshot { get; init; }
}
