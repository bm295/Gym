using System.Security.Claims;
using Gym.Application.Contracts.Tenants;

namespace Gym.Application.Tenants.Context;

/// <summary>
/// Builds the request tenant context exclusively from authenticated claims.
/// A branch_context claim has the form: branch-id|branch-code|branch-name.
/// </summary>
public sealed class ClaimsTenantContextResolver : ITenantContextResolver
{
    public TenantContext Resolve(ClaimsPrincipal principal)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var tenantId = ParseGuidClaim(principal, TenantClaimTypes.TenantId, "tenant ID");
        var tenantName = RequiredClaim(principal, TenantClaimTypes.TenantName, "tenant name");
        var staffUserId = ParseStaffUserId(principal);
        var displayName = RequiredClaim(principal, TenantClaimTypes.StaffDisplayName, "staff display name");
        var role = ParseRole(RequiredClaim(principal, TenantClaimTypes.StaffRole, "staff role"));
        var branches = principal.FindAll(TenantClaimTypes.BranchContext)
            .Select(claim => ParseBranch(claim.Value))
            .ToArray();

        if (branches.Length == 0)
        {
            throw new TenantContextResolutionException("The authenticated staff member has no allowed branch claims.");
        }

        if (branches.Select(branch => branch.BranchId).Distinct().Count() != branches.Length)
        {
            throw new TenantContextResolutionException("The authenticated staff member has duplicate branch claims.");
        }

        return new TenantContext(
            tenantId,
            tenantName,
            new AuthenticatedStaffContext(staffUserId, displayName, role),
            branches);
    }

    private static Guid ParseStaffUserId(ClaimsPrincipal principal)
    {
        var values = principal.FindAll(TenantClaimTypes.StaffUserId)
            .Concat(principal.FindAll("sub"))
            .Select(claim => claim.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (values.Length != 1)
        {
            throw new TenantContextResolutionException(
                values.Length == 0
                    ? "The authenticated principal is missing a staff user ID claim."
                    : "The authenticated principal has conflicting staff user ID claims.");
        }

        return Guid.TryParse(values[0], out var staffUserId)
            ? staffUserId
            : throw new TenantContextResolutionException("The authenticated principal has an invalid staff user ID.");
    }

    private static Guid ParseGuidClaim(ClaimsPrincipal principal, string claimType, string description)
    {
        var value = RequiredClaim(principal, claimType, description);
        return Guid.TryParse(value, out var id)
            ? id
            : throw new TenantContextResolutionException($"The authenticated principal has an invalid {description} claim.");
    }

    private static string RequiredClaim(ClaimsPrincipal principal, string claimType, string description)
    {
        var values = principal.FindAll(claimType)
            .Select(claim => claim.Value?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        return values.Length switch
        {
            1 => values[0]!,
            0 => throw new TenantContextResolutionException(
                $"The authenticated principal is missing a {description} claim."),
            _ => throw new TenantContextResolutionException(
                $"The authenticated principal has conflicting {description} claims.")
        };
    }

    private static TenantStaffRole ParseRole(string value) =>
        Enum.TryParse<TenantStaffRole>(value, ignoreCase: true, out var role)
            ? role
            : throw new TenantContextResolutionException("The authenticated principal has an unsupported staff role.");

    private static AllowedBranchContext ParseBranch(string value)
    {
        var parts = value.Split('|', StringSplitOptions.TrimEntries);
        if (parts.Length != 3
            || !Guid.TryParse(parts[0], out var branchId)
            || string.IsNullOrWhiteSpace(parts[1])
            || string.IsNullOrWhiteSpace(parts[2]))
        {
            throw new TenantContextResolutionException(
                "Each branch_context claim must use 'branch-id|branch-code|branch-name'.");
        }

        return new AllowedBranchContext(branchId, parts[1], parts[2]);
    }
}
