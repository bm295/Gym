using System.Security.Claims;
using Gym.Api.Authorization;
using Gym.Application.Contracts.Tenants;
using Gym.Application.Tenants.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Gym.Tests;

public sealed class TenantAndBranchAccessHandlerTests
{
    private readonly TenantAndBranchAccessRequirement _requirement = new();
    private readonly TenantAndBranchAccessHandler _handler = new(new ClaimsTenantContextResolver());

    [Fact]
    public async Task Allows_an_explicitly_assigned_branch_in_the_authenticated_tenant()
    {
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var authorizationContext = Context(
            Principal(tenantId, TenantStaffRole.BranchManager, branchId),
            tenantId,
            branchId);

        await _handler.HandleAsync(authorizationContext);

        Assert.True(authorizationContext.HasSucceeded);
    }

    [Fact]
    public async Task Allows_a_tenant_admin_to_access_any_branch_in_the_authenticated_tenant()
    {
        var tenantId = Guid.NewGuid();
        var authorizationContext = Context(
            Principal(tenantId, TenantStaffRole.TenantAdmin, Guid.NewGuid()),
            tenantId,
            Guid.NewGuid());

        await _handler.HandleAsync(authorizationContext);

        Assert.True(authorizationContext.HasSucceeded);
    }

    [Theory]
    [InlineData(TenantClaimTypes.TenantId)]
    [InlineData(TenantClaimTypes.StaffUserId)]
    [InlineData(TenantClaimTypes.StaffRole)]
    [InlineData(TenantClaimTypes.BranchContext)]
    public async Task Rejects_missing_required_identity_or_access_claims(string missingClaimType)
    {
        var tenantId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var principal = Principal(tenantId, TenantStaffRole.Receptionist, branchId);
        var claims = principal.Claims.Where(claim => claim.Type != missingClaimType);
        var authorizationContext = Context(Principal(claims), tenantId, branchId);

        await _handler.HandleAsync(authorizationContext);

        Assert.False(authorizationContext.HasSucceeded);
    }

    [Fact]
    public async Task Rejects_a_branch_outside_the_staff_assignments()
    {
        var tenantId = Guid.NewGuid();
        var authorizationContext = Context(
            Principal(tenantId, TenantStaffRole.Receptionist, Guid.NewGuid()),
            tenantId,
            Guid.NewGuid());

        await _handler.HandleAsync(authorizationContext);

        Assert.False(authorizationContext.HasSucceeded);
    }

    [Fact]
    public async Task Rejects_a_route_for_a_different_tenant()
    {
        var authorizationContext = Context(
            Principal(Guid.NewGuid(), TenantStaffRole.TenantAdmin, Guid.NewGuid()),
            Guid.NewGuid(),
            Guid.NewGuid());

        await _handler.HandleAsync(authorizationContext);

        Assert.False(authorizationContext.HasSucceeded);
    }

    private AuthorizationHandlerContext Context(
        ClaimsPrincipal principal,
        Guid tenantId,
        Guid branchId)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues["tenantId"] = tenantId;
        httpContext.Request.RouteValues["branchId"] = branchId;
        return new AuthorizationHandlerContext([_requirement], principal, httpContext);
    }

    private static ClaimsPrincipal Principal(
        Guid tenantId,
        TenantStaffRole role,
        Guid branchId) =>
        Principal(
        [
            new Claim(TenantClaimTypes.TenantId, tenantId.ToString()),
            new Claim(TenantClaimTypes.TenantName, "GymOS Fitness"),
            new Claim(TenantClaimTypes.StaffUserId, Guid.NewGuid().ToString()),
            new Claim(TenantClaimTypes.StaffDisplayName, "Linh Nguyen"),
            new Claim(TenantClaimTypes.StaffRole, role.ToString()),
            new Claim(TenantClaimTypes.BranchContext, $"{branchId}|HCM-01|District 1")
        ]);

    private static ClaimsPrincipal Principal(IEnumerable<Claim> claims) =>
        new(new ClaimsIdentity(claims, "Test"));
}
