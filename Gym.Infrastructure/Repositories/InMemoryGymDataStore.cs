using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryGymDataStore
{
    private readonly object _gate = new();
    private readonly List<Branch> _branches = [];
    private readonly List<Member> _members = [];
    private readonly List<Subscription> _subscriptions = [];
    private readonly List<CheckIn> _checkIns = [];

    public void Add(Branch branch)
    {
        lock (_gate)
        {
            EnsureUniqueId(_branches, branch.Id, nameof(Branch));
            _branches.Add(branch);
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

    public void Add(CheckIn checkIn)
    {
        lock (_gate)
        {
            EnsureUniqueId(_checkIns, checkIn.Id, nameof(CheckIn));
            _checkIns.Add(checkIn);
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
        Branch branch => branch.Id,
        Subscription subscription => subscription.Id,
        CheckIn checkIn => checkIn.Id,
        _ => throw new ArgumentOutOfRangeException(nameof(entity))
    };
}
