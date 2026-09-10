using Gym.Application.Repositories;
using Gym.Infrastructure.Repositories;
using Gym.Tests.Support;

namespace Gym.Tests;

public sealed class InMemoryRepositoryTests
{
    [Fact]
    public void Member_lookup_is_scoped_to_the_tenant_and_accepts_code_or_normalized_phone()
    {
        var tenantId = Guid.NewGuid();
        var member = CheckInSeedData.ActiveMember(tenantId);
        IMemberRepository repository = new InMemoryMemberRepository(new InMemoryGymDataStore());
        repository.Add(member);

        Assert.Same(member, repository.FindByCodeOrNormalizedPhone(tenantId, "mem-001"));
        Assert.Same(member, repository.FindByCodeOrNormalizedPhone(tenantId, member.NormalizedPhoneNumber));
        Assert.Null(repository.FindByCodeOrNormalizedPhone(Guid.NewGuid(), member.MemberCode));
    }

    [Fact]
    public void Branch_lookup_is_scoped_to_the_tenant()
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        IBranchRepository repository = new InMemoryBranchRepository(new InMemoryGymDataStore());
        repository.Add(branch);

        Assert.Same(branch, repository.GetById(tenantId, branch.Id));
        Assert.Null(repository.GetById(Guid.NewGuid(), branch.Id));
    }

    [Fact]
    public void Subscription_and_check_in_repositories_return_only_matching_records()
    {
        var tenantId = Guid.NewGuid();
        var member = CheckInSeedData.ActiveMember(tenantId);
        var branchId = Guid.NewGuid();
        var subscription = CheckInSeedData.ActiveSubscription(tenantId, member.Id, branchId);
        var store = new InMemoryGymDataStore();
        ISubscriptionRepository subscriptions = new InMemorySubscriptionRepository(store);
        ICheckInRepository checkIns = new InMemoryCheckInRepository(store);
        subscriptions.Add(subscription);
        checkIns.Add(CheckInSeedData.CheckIn(tenantId, member.Id, subscription.Id, branchId));

        Assert.Single(subscriptions.GetByMember(tenantId, member.Id));
        Assert.Single(checkIns.GetByMemberAndBranchSince(tenantId, member.Id, branchId, CheckInSeedData.Timestamp.AddMinutes(-5)));
        Assert.Empty(checkIns.GetByMemberAndBranchSince(tenantId, member.Id, branchId, CheckInSeedData.Timestamp.AddMinutes(1)));
    }
}
