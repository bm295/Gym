using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Gym.Application.Contracts.CheckIns;
using Gym.Domain;
using Gym.Infrastructure.Repositories;
using Gym.Tests.Support;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gym.Tests;

public sealed class CheckInApiTests
{
    [Fact]
    public async Task Post_creates_an_unlimited_plan_check_in_in_the_registered_in_memory_store()
    {
        var tenantId = Guid.NewGuid();
        var branch = CheckInSeedData.ActiveBranch(tenantId);
        var member = CheckInSeedData.ActiveMember(tenantId);
        var subscription = CheckInSeedData.ActiveSubscription(tenantId, member.Id, branch.Id);
        await using var factory = new CheckInApiFactory(tenantId, branch.Id);
        Seed(factory, branch, member, subscription);

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            $"/api/tenants/{tenantId}/branches/{branch.Id}/check-ins",
            new CheckInApiRequest(member.MemberCode, "Front desk"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CheckInApiResponse>();
        Assert.NotNull(body);
        Assert.True(body.IsSuccess);
        Assert.NotNull(body.CheckInId);
        Assert.Null(body.RejectionReason);
        Assert.Null(subscription.RemainingVisits);

        var store = factory.Services.GetRequiredService<InMemoryGymDataStore>();
        var checkIn = Assert.Single(store.GetCheckInsByMemberAndBranchSince(
            tenantId, member.Id, branch.Id, DateTimeOffset.MinValue));
        Assert.Equal(body.CheckInId, checkIn.Id);
        Assert.Equal("Front desk", checkIn.Note);
    }

    internal static void Seed(CheckInApiFactory factory, Branch branch, Member member, Subscription subscription)
    {
        var store = factory.Services.GetRequiredService<InMemoryGymDataStore>();
        store.Add(branch);
        store.Add(member);
        store.Add(subscription);
    }
}

internal sealed class CheckInApiFactory(Guid tenantId, Guid branchId) : WebApplicationFactory<Gym.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthenticationHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.SchemeName,
                options =>
                {
                    options.ClaimsIssuer = $"{tenantId}|{branchId}";
                });
        });
    }
}

internal sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    internal const string SchemeName = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var scope = Options.ClaimsIssuer!.Split('|');
        var tenantId = scope[0];
        var branchId = scope[1];
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Receptionist"),
            new Claim("tenant_id", tenantId),
            new Claim("branch_id", branchId)
        ], SchemeName);
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
    }
}
