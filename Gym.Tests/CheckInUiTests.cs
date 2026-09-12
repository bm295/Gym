using Bunit;
using Gym.Application.CheckIns;
using Gym.Application.CheckIns.Contracts;
using Gym.Infrastructure.Repositories;
using Gym.Tests.Support;
using Gym.WebUI.Pages;
using Gym.WebUI.Services.Authentication;
using Gym.WebUI.Services.CheckIns;
using Microsoft.Extensions.DependencyInjection;

namespace Gym.Tests;

public sealed class CheckInUiTests : TestContext
{
    [Fact]
    public void Confirming_an_unlimited_plan_check_in_displays_the_success_state()
    {
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var api = new RecordingCheckInApiClient();
        Services.AddSingleton<ICheckInApiClient>(api);
        var cut = RenderComponent<CheckIn>();

        CompleteAndConfirm(cut, tenantId, branchId);

        Assert.Contains("Check-in successful", cut.Find(".success-result").TextContent);
        Assert.Equal(tenantId, api.TenantId);
        Assert.Equal(branchId, api.BranchId);
        Assert.Equal("MEM-001", api.Request!.MemberCodeOrNormalizedPhone);
    }

    [Fact]
    public async Task Smoke_check_drives_the_ui_through_the_api_into_the_in_memory_store()
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        var member = CheckInSeedData.ActiveMember(tenantId);
        var subscription = CheckInSeedData.ActiveSubscription(tenantId, member.Id, branch.Id);
        await using var factory = new CheckInApiFactory(tenantId, branch.Id);
        CheckInApiTests.Seed(factory, branch, member, subscription);

        Services.AddSingleton<IAccessTokenProvider>(new StaticAccessTokenProvider());
        Services.AddSingleton<ICheckInApiClient>(_ => new AuthenticatedCheckInApiClient(
            factory.CreateClient(), Services.GetRequiredService<IAccessTokenProvider>()));
        var cut = RenderComponent<CheckIn>();

        CompleteAndConfirm(cut, tenantId, branch.Id);
        cut.WaitForAssertion(() => Assert.NotNull(cut.Find(".success-result")));

        var store = factory.Services.GetRequiredService<InMemoryGymDataStore>();
        Assert.Single(store.GetCheckInsByMemberAndBranchSince(
            tenantId, member.Id, branch.Id, DateTimeOffset.MinValue));
        Assert.Null(subscription.RemainingVisits);
    }

    private static void CompleteAndConfirm(IRenderedComponent<CheckIn> cut, Guid tenantId, Guid branchId)
    {
        cut.Find("#member-lookup").Input("MEM-001");
        cut.Find("#tenant-id").Input(tenantId.ToString());
        cut.Find("#branch-id").Input(branchId.ToString());
        cut.Find("form").Submit();
        cut.Find("button.btn-success").Click();
    }

    private sealed class RecordingCheckInApiClient : ICheckInApiClient
    {
        public Guid TenantId { get; private set; }
        public Guid BranchId { get; private set; }
        public CheckInApiRequest? Request { get; private set; }

        public Task<CheckInApiResponse> CheckInAsync(Guid tenantId, Guid branchId, CheckInApiRequest request, CancellationToken cancellationToken = default)
        {
            TenantId = tenantId;
            BranchId = branchId;
            Request = request;
            return Task.FromResult(new CheckInApiResponse(true, Guid.NewGuid(), null));
        }
    }

    private sealed class StaticAccessTokenProvider : IAccessTokenProvider
    {
        public ValueTask<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default) => ValueTask.FromResult<string?>("test-token");
    }
}
