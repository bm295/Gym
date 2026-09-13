using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Gym.Application.Contracts.Tenants;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gym.Tests;

public sealed class ContextApiTests
{
    [Fact]
    public async Task Get_returns_the_authenticated_staff_tenant_context_and_allowed_branches()
    {
        var tenantId = Guid.NewGuid();
        var staffUserId = Guid.NewGuid();
        var firstBranchId = Guid.NewGuid();
        var secondBranchId = Guid.NewGuid();
        await using var factory = new ContextApiFactory(
        [
            new Claim(TenantClaimTypes.TenantId, tenantId.ToString()),
            new Claim(TenantClaimTypes.TenantName, "GymOS Fitness"),
            new Claim(ClaimTypes.NameIdentifier, staffUserId.ToString()),
            new Claim(ClaimTypes.Name, "Linh Nguyen"),
            new Claim(ClaimTypes.Role, nameof(TenantStaffRole.BranchManager)),
            new Claim(TenantClaimTypes.BranchContext, $"{firstBranchId}|HCM-01|District 1"),
            new Claim(TenantClaimTypes.BranchContext, $"{secondBranchId}|HCM-02|District 3")
        ]);

        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/context");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var context = await response.Content.ReadFromJsonAsync<TenantContext>();
        Assert.NotNull(context);
        Assert.Equal(tenantId, context.TenantId);
        Assert.Equal("GymOS Fitness", context.TenantName);
        Assert.Equal(staffUserId, context.Staff.StaffUserId);
        Assert.Equal("Linh Nguyen", context.Staff.DisplayName);
        Assert.Equal(TenantStaffRole.BranchManager, context.Staff.Role);
        Assert.Equal(
        [
            new AllowedBranchContext(firstBranchId, "HCM-01", "District 1"),
            new AllowedBranchContext(secondBranchId, "HCM-02", "District 3")
        ], context.AllowedBranches);
    }

    [Fact]
    public async Task Get_rejects_an_unauthenticated_request()
    {
        await using var factory = new WebApplicationFactory<Gym.Api.Program>();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/api/context");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}

internal sealed class ContextApiFactory(IReadOnlyCollection<Claim> claims)
    : WebApplicationFactory<Gym.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ContextAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = ContextAuthenticationHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, ContextAuthenticationHandler>(
                ContextAuthenticationHandler.SchemeName,
                options => options.ClaimsIssuer = "Context API tests");
            services.AddSingleton(claims);
        });
    }
}

internal sealed class ContextAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IReadOnlyCollection<Claim> claims)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    internal const string SchemeName = "ContextTest";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(claims, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(
            new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
    }
}
