namespace Gym.WebUI.Services.Prototype;

public sealed class PrototypeGymState : IPrototypeGymState
{
    private readonly List<PrototypeBranch> _branches;
    private readonly List<PrototypeStaff> _staff;
    private readonly List<PrototypeMember> _members;
    private readonly List<PrototypePlan> _plans;
    private readonly List<PrototypeSubscription> _subscriptions;
    private readonly List<PrototypePayment> _payments;
    private readonly List<PrototypeCheckIn> _checkIns;
    private readonly List<PrototypeAuditEntry> _auditEntries = [];

    public PrototypeGymState()
    {
        Tenant = new PrototypeTenant(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "GymOS Fitness", "GYMOS");
        _branches =
        [
            new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "HCM-01", "District 1", "12 Nguyen Hue, District 1", true),
            new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "HCM-02", "Thao Dien", "28 Xuan Thuy, Thu Duc", true)
        ];
        _staff =
        [
            new(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Linh Nguyen", "Receptionist", _branches[0].Id, true),
            new(Guid.Parse("44444444-4444-4444-4444-444444444444"), "Minh Tran", "Branch Manager", _branches[0].Id, true)
        ];
        _members =
        [
            new() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), MemberCode = "MEM-1001", FullName = "Nguyen An", PhoneNumber = "0901234567", HomeBranchId = _branches[0].Id, Status = "Active", JoinedOn = DateOnly.FromDateTime(DateTime.Today).AddMonths(-4) },
            new() { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), MemberCode = "MEM-1002", FullName = "Tran Ha", PhoneNumber = "0902345678", HomeBranchId = _branches[1].Id, Status = "Active", JoinedOn = DateOnly.FromDateTime(DateTime.Today).AddMonths(-2) },
            new() { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), MemberCode = "MEM-1003", FullName = "Le Bao", PhoneNumber = "0903456789", HomeBranchId = _branches[0].Id, Status = "Suspended", JoinedOn = DateOnly.FromDateTime(DateTime.Today).AddMonths(-8) },
            new() { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004"), MemberCode = "MEM-1004", FullName = "Pham Dao", PhoneNumber = "0904567890", HomeBranchId = _branches[0].Id, Status = "Active", JoinedOn = DateOnly.FromDateTime(DateTime.Today).AddMonths(-1) },
            new() { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005"), MemberCode = "MEM-1005", FullName = "Vo Giang", PhoneNumber = "0905678901", HomeBranchId = _branches[0].Id, Status = "Active", JoinedOn = DateOnly.FromDateTime(DateTime.Today).AddMonths(-3) },
            new() { Id = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006"), MemberCode = "MEM-1006", FullName = "Bui Kim", PhoneNumber = "0906789012", HomeBranchId = _branches[0].Id, Status = "Active", JoinedOn = DateOnly.FromDateTime(DateTime.Today).AddMonths(-3) }
        ];
        _plans =
        [
            new() { Id = Guid.Parse("88888888-8888-8888-8888-888888888888"), Code = "MONTHLY-U", Name = "Monthly Unlimited", DurationDays = 30, VisitLimit = null, Price = 500_000m, AllowCrossBranch = false, Status = "Active" },
            new() { Id = Guid.Parse("99999999-9999-9999-9999-999999999999"), Code = "PT-12", Name = "12 Visit Pack", DurationDays = 60, VisitLimit = 12, Price = 850_000m, AllowCrossBranch = true, Status = "Active" }
        ];
        var now = DateTimeOffset.UtcNow;
        _subscriptions =
        [
            new() { Id = Guid.NewGuid(), MemberId = _members[0].Id, PlanId = _plans[0].Id, HomeBranchId = _branches[0].Id, PlanName = _plans[0].Name, Status = "Active", StartDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(-5), EndDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(25), RemainingVisits = null, AllowCrossBranch = false, SalePrice = _plans[0].Price },
            new() { Id = Guid.NewGuid(), MemberId = _members[1].Id, PlanId = _plans[1].Id, HomeBranchId = _branches[1].Id, PlanName = _plans[1].Name, Status = "Active", StartDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(-10), EndDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(50), RemainingVisits = 8, AllowCrossBranch = true, SalePrice = _plans[1].Price },
            new() { Id = Guid.NewGuid(), MemberId = _members[4].Id, PlanId = _plans[0].Id, HomeBranchId = _branches[0].Id, PlanName = _plans[0].Name, Status = "Active", StartDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(-40), EndDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(-10), RemainingVisits = null, AllowCrossBranch = false, SalePrice = _plans[0].Price },
            new() { Id = Guid.NewGuid(), MemberId = _members[5].Id, PlanId = _plans[1].Id, HomeBranchId = _branches[0].Id, PlanName = _plans[1].Name, Status = "Active", StartDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(-5), EndDate = DateOnly.FromDateTime(now.UtcDateTime).AddDays(55), RemainingVisits = 0, AllowCrossBranch = false, SalePrice = _plans[1].Price }
        ];
        _payments =
        [
            new() { Id = Guid.NewGuid(), PaymentNo = "PAY-24001", MemberId = _members[0].Id, SubscriptionId = _subscriptions[0].Id, Amount = 500_000m, Method = "Cash", PaidAt = now.AddDays(-5) },
            new() { Id = Guid.NewGuid(), PaymentNo = "PAY-24002", MemberId = _members[1].Id, SubscriptionId = _subscriptions[1].Id, Amount = 850_000m, Method = "Bank transfer", PaidAt = now.AddDays(-10) }
        ];
        _checkIns = [new PrototypeCheckIn(Guid.NewGuid(), _members[0].Id, _subscriptions[0].Id, _branches[0].Id, now.AddHours(-2))];
        Reports = [new("Active members", "248", "+12 this month", "success"), new("Check-ins today", "86", "+9% vs yesterday", "info"), new("Revenue today", "12.4m VND", "+6% vs yesterday", "success"), new("Expiring this week", "9", "Needs follow-up", "warning")];
    }

    public event Action? Changed;
    public PrototypeTenant Tenant { get; }
    public IReadOnlyList<PrototypeBranch> Branches => _branches;
    public IReadOnlyList<PrototypeStaff> Staff => _staff;
    public IReadOnlyList<PrototypeMember> Members => _members;
    public IReadOnlyList<PrototypePlan> Plans => _plans;
    public IReadOnlyList<PrototypeSubscription> Subscriptions => _subscriptions;
    public IReadOnlyList<PrototypePayment> Payments => _payments;
    public IReadOnlyList<PrototypeCheckIn> CheckIns => _checkIns;
    public IReadOnlyList<PrototypeAuditEntry> AuditEntries => _auditEntries;
    public IReadOnlyList<PrototypeReportMetric> Reports { get; }

    public PrototypeMember CreateMember(MemberDraft draft)
    {
        var member = new PrototypeMember { Id = Guid.NewGuid(), MemberCode = $"MEM-{1001 + _members.Count}", FullName = draft.FullName.Trim(), PhoneNumber = draft.PhoneNumber.Trim(), HomeBranchId = draft.HomeBranchId, Status = "Active", JoinedOn = DateOnly.FromDateTime(DateTime.Today) };
        _members.Add(member); Audit("Create member", $"Created member {member.FullName}"); Notify(); return member;
    }
    public bool EditMember(Guid memberId, MemberDraft draft)
    {
        var member = _members.SingleOrDefault(item => item.Id == memberId); if (member is null) return false;
        member.FullName = draft.FullName.Trim(); member.PhoneNumber = draft.PhoneNumber.Trim(); member.HomeBranchId = draft.HomeBranchId; Audit("Edit member", $"Updated member {member.FullName}"); Notify(); return true;
    }
    public bool SetMemberLocked(Guid memberId, bool isLocked)
    {
        var member = _members.SingleOrDefault(item => item.Id == memberId); if (member is null) return false;
        member.Status = isLocked ? "Suspended" : "Active"; Audit(isLocked ? "Lock member" : "Unlock member", member.FullName); Notify(); return true;
    }
    public bool ArchivePlan(Guid planId)
    {
        var plan = _plans.SingleOrDefault(item => item.Id == planId); if (plan is null) return false;
        plan.Status = "Archived"; Audit("Archive plan", plan.Name); Notify(); return true;
    }
    public PrototypeSubscription? SellSubscription(SellSubscriptionDraft draft)
    {
        var member = _members.SingleOrDefault(item => item.Id == draft.MemberId); var plan = _plans.SingleOrDefault(item => item.Id == draft.PlanId && item.Status == "Active");
        if (member is null || plan is null) return null;
        var subscription = new PrototypeSubscription { Id = Guid.NewGuid(), MemberId = member.Id, PlanId = plan.Id, HomeBranchId = draft.HomeBranchId, PlanName = plan.Name, Status = "Active", StartDate = draft.StartDate, EndDate = draft.StartDate.AddDays(plan.DurationDays - 1), RemainingVisits = plan.VisitLimit, AllowCrossBranch = plan.AllowCrossBranch, SalePrice = plan.Price };
        _subscriptions.Add(subscription); if (draft.AmountPaid > 0) RecordPayment(new PaymentDraft(member.Id, subscription.Id, draft.AmountPaid, "Cash")); Audit("Sell subscription", $"Sold {plan.Name} to {member.FullName}"); Notify(); return subscription;
    }
    public PrototypeSubscription? RenewSubscription(Guid subscriptionId, Guid planId, DateOnly startDate)
    {
        var previous = _subscriptions.SingleOrDefault(item => item.Id == subscriptionId); if (previous is null) return null;
        return SellSubscription(new SellSubscriptionDraft(previous.MemberId, planId, previous.HomeBranchId, startDate, 0));
    }
    public PrototypePayment? RecordPayment(PaymentDraft draft)
    {
        if (!_members.Any(member => member.Id == draft.MemberId) || draft.Amount <= 0) return null;
        var payment = new PrototypePayment { Id = Guid.NewGuid(), PaymentNo = $"PAY-{24000 + _payments.Count + 1}", MemberId = draft.MemberId, SubscriptionId = draft.SubscriptionId, Amount = draft.Amount, Method = draft.Method, PaidAt = DateTimeOffset.UtcNow };
        _payments.Add(payment); Audit("Record payment", $"Recorded {payment.Amount:N0} VND"); Notify(); return payment;
    }
    public bool VoidPayment(Guid paymentId, string reason)
    {
        var payment = _payments.SingleOrDefault(item => item.Id == paymentId); if (payment is null || payment.IsVoided || string.IsNullOrWhiteSpace(reason)) return false;
        payment.IsVoided = true; payment.VoidReason = reason.Trim(); Audit("Void payment", $"Voided {payment.PaymentNo}"); Notify(); return true;
    }
    public PrototypeCheckInResult CheckIn(Guid memberId, Guid branchId)
    {
        var member = _members.SingleOrDefault(item => item.Id == memberId); if (member?.Status != "Active") return new(false, "Member is not active.", null);
        var today = DateOnly.FromDateTime(DateTime.UtcNow); var subscription = _subscriptions.LastOrDefault(item => item.MemberId == memberId && item.Status == "Active" && item.StartDate <= today && item.EndDate >= today);
        if (subscription is null) return new(false, "No valid active subscription.", null);
        if (subscription.HomeBranchId != branchId && !subscription.AllowCrossBranch) return new(false, "This plan is not valid at the selected branch.", null);
        if (subscription.RemainingVisits == 0) return new(false, "No visits remaining.", null);
        if (_checkIns.Any(item => item.MemberId == memberId && item.BranchId == branchId && item.CheckedInAt >= DateTimeOffset.UtcNow.AddMinutes(-5))) return new(false, "A recent check-in already exists.", null);
        if (subscription.RemainingVisits is not null) subscription.RemainingVisits--;
        var checkIn = new PrototypeCheckIn(Guid.NewGuid(), memberId, subscription.Id, branchId, DateTimeOffset.UtcNow); _checkIns.Add(checkIn); Audit("Check-in", $"Checked in {member.FullName}"); Notify(); return new(true, "Check-in recorded for this session.", checkIn);
    }
    private void Audit(string action, string summary) => _auditEntries.Insert(0, new PrototypeAuditEntry(Guid.NewGuid(), DateTimeOffset.UtcNow, action, summary, "Linh Nguyen"));
    private void Notify() => Changed?.Invoke();
}
