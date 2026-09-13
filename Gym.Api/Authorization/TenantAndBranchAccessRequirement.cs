using Gym.Application.Contracts.Tenants;
using Gym.Application.Tenants.Access;
using Gym.Application.Tenants.Context;
using Microsoft.AspNetCore.Authorization;

namespace Gym.Api.Authorization;

public sealed class TenantAndBranchAccessRequirement : IAuthorizationRequirement;

public sealed class TenantAndBranchAccessHandler(ITenantContextResolver tenantContextResolver)
    : AuthorizationHandler<TenantAndBranchAccessRequirement>
{
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

        TenantContext tenantContext;
        try
        {
            tenantContext = tenantContextResolver.Resolve(context.User);
        }
        catch (TenantContextResolutionException)
        {
            return Task.CompletedTask;
        }

        var branchAccess = tenantContext.Staff.Role switch
        {
            TenantStaffRole.TenantAdmin => BranchAccess.TenantAdmin(),
            TenantStaffRole.BranchManager => BranchAccess.BranchManager(
                tenantContext.AllowedBranches.Select(branch => branch.BranchId)),
            TenantStaffRole.Receptionist => BranchAccess.Receptionist(
                tenantContext.AllowedBranches.Select(branch => branch.BranchId)),
            _ => null
        };

        if (tenantContext.TenantId == tenantId && branchAccess?.CanAccess(branchId) == true)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
