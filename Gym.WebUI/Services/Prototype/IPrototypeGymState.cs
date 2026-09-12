namespace Gym.WebUI.Services.Prototype;

/// <summary>Client-side demonstration state. It deliberately does not call API clients or persist remotely.</summary>
public interface IPrototypeGymState
{
    event Action? Changed;
    PrototypeTenant Tenant { get; }
    IReadOnlyList<PrototypeBranch> Branches { get; }
    IReadOnlyList<PrototypeStaff> Staff { get; }
    IReadOnlyList<PrototypeMember> Members { get; }
    IReadOnlyList<PrototypePlan> Plans { get; }
    IReadOnlyList<PrototypeSubscription> Subscriptions { get; }
    IReadOnlyList<PrototypePayment> Payments { get; }
    IReadOnlyList<PrototypeCheckIn> CheckIns { get; }
    IReadOnlyList<PrototypeAuditEntry> AuditEntries { get; }
    IReadOnlyList<PrototypeReportMetric> Reports { get; }
    PrototypeMember CreateMember(MemberDraft draft);
    bool EditMember(Guid memberId, MemberDraft draft);
    bool SetMemberLocked(Guid memberId, bool isLocked);
    bool ArchivePlan(Guid planId);
    PrototypeSubscription? SellSubscription(SellSubscriptionDraft draft);
    PrototypeSubscription? RenewSubscription(Guid subscriptionId, Guid planId, DateOnly startDate);
    PrototypePayment? RecordPayment(PaymentDraft draft);
    bool VoidPayment(Guid paymentId, string reason);
    PrototypeCheckInResult CheckIn(Guid memberId, Guid branchId);
}

public sealed record PrototypeTenant(Guid Id, string Name, string Code);
public sealed record PrototypeBranch(Guid Id, string Code, string Name, string Address, bool IsActive);
public sealed record PrototypeStaff(Guid Id, string Name, string Role, Guid BranchId, bool IsActive);
public sealed class PrototypeMember
{
    public required Guid Id { get; init; }
    public required string MemberCode { get; set; }
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public required Guid HomeBranchId { get; set; }
    public required string Status { get; set; }
    public DateOnly JoinedOn { get; init; }
}
public sealed class PrototypePlan
{
    public required Guid Id { get; init; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required int DurationDays { get; set; }
    public int? VisitLimit { get; set; }
    public required decimal Price { get; set; }
    public bool AllowCrossBranch { get; set; }
    public required string Status { get; set; }
}
public sealed class PrototypeSubscription
{
    public required Guid Id { get; init; }
    public required Guid MemberId { get; init; }
    public required Guid PlanId { get; init; }
    public required Guid HomeBranchId { get; init; }
    public required string PlanName { get; init; }
    public required string Status { get; set; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly EndDate { get; init; }
    public int? RemainingVisits { get; set; }
    public bool AllowCrossBranch { get; init; }
    public decimal SalePrice { get; init; }
}
public sealed class PrototypePayment
{
    public required Guid Id { get; init; }
    public required string PaymentNo { get; init; }
    public required Guid MemberId { get; init; }
    public Guid? SubscriptionId { get; init; }
    public decimal Amount { get; init; }
    public required string Method { get; init; }
    public DateTimeOffset PaidAt { get; init; }
    public bool IsVoided { get; set; }
    public string? VoidReason { get; set; }
}
public sealed record PrototypeCheckIn(Guid Id, Guid MemberId, Guid SubscriptionId, Guid BranchId, DateTimeOffset CheckedInAt);
public sealed record PrototypeAuditEntry(Guid Id, DateTimeOffset OccurredAt, string Action, string Summary, string PerformedBy);
public sealed record PrototypeReportMetric(string Label, string Value, string Trend, string Tone);
public sealed class MemberDraft
{
    public MemberDraft(string fullName, string phoneNumber, Guid homeBranchId)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        HomeBranchId = homeBranchId;
    }

    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public Guid HomeBranchId { get; set; }
}
public sealed class SellSubscriptionDraft
{
    public SellSubscriptionDraft(Guid memberId, Guid planId, Guid homeBranchId, DateOnly startDate, decimal amountPaid)
    {
        MemberId = memberId; PlanId = planId; HomeBranchId = homeBranchId; StartDate = startDate; AmountPaid = amountPaid;
    }
    public Guid MemberId { get; set; }
    public Guid PlanId { get; set; }
    public Guid HomeBranchId { get; set; }
    public DateOnly StartDate { get; set; }
    public decimal AmountPaid { get; set; }
}
public sealed record PaymentDraft(Guid MemberId, Guid? SubscriptionId, decimal Amount, string Method);
public sealed record PrototypeCheckInResult(bool IsSuccess, string Message, PrototypeCheckIn? CheckIn);
