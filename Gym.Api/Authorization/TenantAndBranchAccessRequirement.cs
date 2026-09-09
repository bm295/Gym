using Microsoft.AspNetCore.Authorization;

namespace Gym.Api.Authorization;

public sealed class TenantAndBranchAccessRequirement : IAuthorizationRequirement;

public sealed class TenantAndBranchAccessHandler
    : AuthorizationHandler<TenantAndBranchAccessRequirement>
{
    public const string TenantClaimType = "tenant_id";
    public const string BranchClaimType = "branch_id";

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantAndBranchAccessRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            return Task.CompletedTask;
        }

        if (!Guid.TryParse(httpContext.Request.RouteValues["tenantId"]?.ToString(), out var tenantId)
            || !Guid.TryParse(httpContext.Request.RouteValues["branchId"]?.ToString(), out var branchId))
        {
            return Task.CompletedTask;
        }

        var canAccessTenant = HasIdClaim(context, TenantClaimType, tenantId);
        var canAccessBranch = context.User.IsInRole(StaffRoles.TenantAdmin)
            || HasIdClaim(context, BranchClaimType, branchId);

        if (canAccessTenant && canAccessBranch)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool HasIdClaim(AuthorizationHandlerContext context, string claimType, Guid id) =>
        context.User.Claims.Any(claim =>
            claim.Type == claimType
            && Guid.TryParse(claim.Value, out var claimId)
            && claimId == id);
}
