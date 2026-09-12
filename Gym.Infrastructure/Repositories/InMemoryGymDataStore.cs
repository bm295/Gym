using Gym.Domain;
using Gym.Infrastructure.SeedData;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryGymDataStore
{
    private readonly object _gate = new();
    private readonly List<Tenant> _tenants = [];
    private readonly List<Branch> _branches = [];
    private readonly List<StaffUser> _staffUsers = [];
    private readonly List<StaffBranchAccessAssignment> _staffBranchAccessAssignments = [];
    private readonly List<MembershipPlan> _membershipPlans = [];
    private readonly List<Member> _members = [];
    private readonly List<Subscription> _subscriptions = [];
    private readonly List<Payment> _payments = [];
    private readonly List<CheckIn> _checkIns = [];
    private readonly List<AuditEntry> _auditEntries = [];

    public InMemoryGymDataStore() => InMemoryGymDataSeeder.Seed(this);

    public void Add(Tenant tenant)
    {
        lock (_gate)
        {
            EnsureUniqueId(_tenants, tenant.Id, nameof(Tenant));
            _tenants.Add(tenant);
        }
    }

    public void Add(Branch branch)
    {
        lock (_gate)
        {
            EnsureUniqueId(_branches, branch.Id, nameof(Branch));
            _branches.Add(branch);
        }
    }

    public void Add(StaffUser staffUser)
    {
        lock (_gate)
        {
            EnsureUniqueId(_staffUsers, staffUser.Id, nameof(StaffUser));
            _staffUsers.Add(staffUser);
        }
    }

    public void Add(StaffBranchAccessAssignment assignment)
    {
        lock (_gate)
        {
            if (_staffBranchAccessAssignments.Any(candidate =>
                candidate.TenantId == assignment.TenantId
                && candidate.StaffUserId == assignment.StaffUserId
                && candidate.BranchId == assignment.BranchId))
            {
                throw new InvalidOperationException("The staff branch access assignment already exists.");
            }

            _staffBranchAccessAssignments.Add(assignment);
        }
    }

    public void Add(MembershipPlan plan)
    {
        lock (_gate)
        {
            EnsureUniqueId(_membershipPlans, plan.Id, nameof(MembershipPlan));
            _membershipPlans.Add(plan);
        }
    }

    public void Add(Member member)
    {
        lock (_gate)
        {
            EnsureUniqueId(_members, member.Id, nameof(Member));
            _members.Add(member);
        }
    }

    public void Add(Subscription subscription)
    {
        lock (_gate)
        {
            EnsureUniqueId(_subscriptions, subscription.Id, nameof(Subscription));
            _subscriptions.Add(subscription);
        }
    }

    public void Add(Payment payment)
    {
        lock (_gate)
        {
            EnsureUniqueId(_payments, payment.Id, nameof(Payment));
            _payments.Add(payment);
        }
    }

    public void Add(CheckIn checkIn)
    {
        lock (_gate)
        {
            EnsureUniqueId(_checkIns, checkIn.Id, nameof(CheckIn));
            _checkIns.Add(checkIn);
        }
    }

    public void Add(AuditEntry entry)
    {
        lock (_gate)
        {
            EnsureUniqueId(_auditEntries, entry.Id, nameof(AuditEntry));
            _auditEntries.Add(entry);
        }
    }

    public Member? FindMemberByCodeOrNormalizedPhone(Guid tenantId, string lookup)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lookup);

        lock (_gate)
        {
            return _members.SingleOrDefault(member =>
                member.TenantId == tenantId
                && !member.IsDeleted
                && (string.Equals(member.MemberCode, lookup, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(member.NormalizedPhoneNumber, lookup, StringComparison.Ordinal)));
        }
    }

    public Branch? GetBranchById(Guid tenantId, Guid branchId)
    {
        lock (_gate)
        {
            return _branches.SingleOrDefault(branch =>
                branch.TenantId == tenantId && branch.Id == branchId && !branch.IsDeleted);
        }
    }

    public IReadOnlyList<Tenant> GetTenants()
    {
        lock (_gate)
        {
            return _tenants.ToArray();
        }
    }

    public IReadOnlyList<Branch> GetBranches(Guid tenantId)
    {
        lock (_gate)
        {
            return _branches.Where(branch => branch.TenantId == tenantId && !branch.IsDeleted).ToArray();
        }
    }

    public IReadOnlyList<StaffUser> GetStaffUsers(Guid tenantId)
    {
        lock (_gate)
        {
            return _staffUsers.Where(staffUser => staffUser.TenantId == tenantId).ToArray();
        }
    }

    public IReadOnlyList<StaffBranchAccessAssignment> GetBranchAccessAssignments(Guid tenantId, Guid staffUserId)
    {
        lock (_gate)
        {
            return _staffBranchAccessAssignments
                .Where(assignment => assignment.TenantId == tenantId && assignment.StaffUserId == staffUserId)
                .ToArray();
        }
    }

    public IReadOnlyList<MembershipPlan> GetMembershipPlans(Guid tenantId)
    {
        lock (_gate)
        {
            return _membershipPlans.Where(plan => plan.TenantId == tenantId && !plan.IsDeleted).ToArray();
        }
    }

    public IReadOnlyList<Member> GetMembers(Guid tenantId)
    {
        lock (_gate)
        {
            return _members.Where(member => member.TenantId == tenantId && !member.IsDeleted).ToArray();
        }
    }

    public IReadOnlyList<Payment> GetPayments(Guid tenantId)
    {
        lock (_gate)
        {
            return _payments.Where(payment => payment.TenantId == tenantId).ToArray();
        }
    }

    public IReadOnlyList<Subscription> GetSubscriptions(Guid tenantId)
    {
        lock (_gate)
        {
            return _subscriptions.Where(subscription => subscription.TenantId == tenantId).ToArray();
        }
    }

    public IReadOnlyList<CheckIn> GetCheckIns(Guid tenantId)
    {
        lock (_gate)
        {
            return _checkIns.Where(checkIn => checkIn.TenantId == tenantId).ToArray();
        }
    }

    public IReadOnlyList<AuditEntry> GetAuditEntries(Guid tenantId)
    {
        lock (_gate)
        {
            return _auditEntries.Where(entry => entry.TenantId == tenantId).ToArray();
        }
    }

    public IReadOnlyList<Subscription> GetSubscriptionsByMember(Guid tenantId, Guid memberId)
    {
        lock (_gate)
        {
            return _subscriptions
                .Where(subscription => subscription.TenantId == tenantId && subscription.MemberId == memberId)
                .ToArray();
        }
    }

    public IReadOnlyList<CheckIn> GetCheckInsByMemberAndBranchSince(
        Guid tenantId,
        Guid memberId,
        Guid branchId,
        DateTimeOffset since)
    {
        lock (_gate)
        {
            return _checkIns
                .Where(checkIn =>
                    checkIn.TenantId == tenantId
                    && checkIn.MemberId == memberId
                    && checkIn.BranchId == branchId
                    && checkIn.CheckedInAt >= since)
                .ToArray();
        }
    }

    private static void EnsureUniqueId<T>(IEnumerable<T> entities, Guid id, string entityName)
        where T : class
    {
        if (entities.Any(entity => GetId(entity) == id))
        {
            throw new InvalidOperationException($"A {entityName} with id '{id}' already exists.");
        }
    }

    private static Guid GetId<T>(T entity) where T : class => entity switch
    {
        Member member => member.Id,
        Tenant tenant => tenant.Id,
        Branch branch => branch.Id,
        StaffUser staffUser => staffUser.Id,
        MembershipPlan plan => plan.Id,
        Subscription subscription => subscription.Id,
        Payment payment => payment.Id,
        CheckIn checkIn => checkIn.Id,
        AuditEntry entry => entry.Id,
        _ => throw new ArgumentOutOfRangeException(nameof(entity))
    };
}
