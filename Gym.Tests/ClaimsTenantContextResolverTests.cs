using System.Security.Claims;
using Gym.Application.Contracts.Tenants;
using Gym.Application.Tenants.Context;

namespace Gym.Tests;

public sealed class ClaimsTenantContextResolverTests
{
    private readonly ClaimsTenantContextResolver _resolver = new();

    [Fact]
    public void Resolves_context_only_from_authenticated_claims()
    {
        var tenantId = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var principal = Principal(
            new Claim(TenantClaimTypes.TenantId, tenantId.ToString()),
            new Claim(TenantClaimTypes.TenantName, "GymOS Fitness"),
            new Claim(ClaimTypes.NameIdentifier, staffId.ToString()),
            new Claim(ClaimTypes.Name, "Linh Nguyen"),
            new Claim(ClaimTypes.Role, nameof(TenantStaffRole.Receptionist)),
            new Claim(TenantClaimTypes.BranchContext, $"{branchId}|HCM-01|District 1"));

        var context = _resolver.Resolve(principal);

        Assert.Equal(tenantId, context.TenantId);
        Assert.Equal("GymOS Fitness", context.TenantName);
        Assert.Equal(staffId, context.Staff.StaffUserId);
        Assert.Equal(TenantStaffRole.Receptionist, context.Staff.Role);
        Assert.Equal(new AllowedBranchContext(branchId, "HCM-01", "District 1"), context.AllowedBranches.Single());
    }

    [Theory]
    [InlineData(TenantClaimTypes.TenantId)]
    [InlineData(TenantClaimTypes.TenantName)]
    [InlineData(ClaimTypes.NameIdentifier)]
    [InlineData(ClaimTypes.Name)]
    [InlineData(ClaimTypes.Role)]
    [InlineData(TenantClaimTypes.BranchContext)]
    public void Rejects_a_principal_missing_required_context_claims(string missingClaimType)
    {
        var claims = ValidClaims().Where(claim => claim.Type != missingClaimType);

        Assert.Throws<TenantContextResolutionException>(() => _resolver.Resolve(Principal(claims.ToArray())));
    }

    [Fact]
    public void Rejects_malformed_branch_context_claims()
    {
        var claims = ValidClaims().Where(claim => claim.Type != TenantClaimTypes.BranchContext)
            .Append(new Claim(TenantClaimTypes.BranchContext, "not-a-branch"));

        Assert.Throws<TenantContextResolutionException>(() => _resolver.Resolve(Principal(claims.ToArray())));
    }

    private static Claim[] ValidClaims() =>
    [
        new Claim(TenantClaimTypes.TenantId, Guid.NewGuid().ToString()),
        new Claim(TenantClaimTypes.TenantName, "GymOS Fitness"),
        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, "Linh Nguyen"),
        new Claim(ClaimTypes.Role, nameof(TenantStaffRole.Receptionist)),
        new Claim(TenantClaimTypes.BranchContext, $"{Guid.NewGuid()}|HCM-01|District 1")
    ];

    private static ClaimsPrincipal Principal(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "Test"));
}
