using Gym.WebUI.Services.Prototype;

namespace Gym.Tests;

public sealed class PrototypeStateTests
{
    [Fact]
    public void Prototype_mutations_update_only_the_in_memory_session_state()
    {
        var state = new PrototypeGymState();
        var member = state.CreateMember(new MemberDraft("Demo Member", "0999999999", state.Branches[0].Id));
        var plan = state.Plans.First(plan => plan.Status == "Active");

        var subscription = state.SellSubscription(new SellSubscriptionDraft(member.Id, plan.Id, state.Branches[0].Id, DateOnly.FromDateTime(DateTime.Today), plan.Price));
        var payment = state.Payments.Last();

        Assert.NotNull(subscription);
        Assert.True(state.VoidPayment(payment.Id, "Prototype correction"));
        Assert.True(payment.IsVoided);
        Assert.Contains(state.AuditEntries, entry => entry.Action == "Create member");
    }

    [Fact]
    public void Prototype_check_in_covers_wrong_branch_and_duplicate_rejections()
    {
        var state = new PrototypeGymState();
        var unlimited = state.Members.Single(member => member.MemberCode == "MEM-1001");
        var wrongBranch = state.Branches[1];

        var rejected = state.CheckIn(unlimited.Id, wrongBranch.Id);
        var successful = state.CheckIn(unlimited.Id, state.Branches[0].Id);
        var duplicate = state.CheckIn(unlimited.Id, state.Branches[0].Id);

        Assert.False(rejected.IsSuccess);
        Assert.Contains("branch", rejected.Message, StringComparison.OrdinalIgnoreCase);
        Assert.True(successful.IsSuccess);
        Assert.False(duplicate.IsSuccess);
        Assert.Contains("recent", duplicate.Message, StringComparison.OrdinalIgnoreCase);
    }
}
