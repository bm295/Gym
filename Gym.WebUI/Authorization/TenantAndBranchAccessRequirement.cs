using Microsoft.AspNetCore.Authorization;

namespace Gym.WebUI.Authorization;

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

        var routeTenantId = httpContext.Request.RouteValues["tenantId"]?.ToString();
        var routeBranchId = httpContext.Request.RouteValues["branchId"]?.ToString();
        if (routeTenantId is null || routeBranchId is null)
        {
            return Task.CompletedTask;
        }

        var canAccessTenant = context.User.HasClaim(TenantClaimType, routeTenantId);
        var canAccessBranch = context.User.IsInRole(StaffRoles.TenantAdmin)
            || context.User.HasClaim(BranchClaimType, routeBranchId);

        if (canAccessTenant && canAccessBranch)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
