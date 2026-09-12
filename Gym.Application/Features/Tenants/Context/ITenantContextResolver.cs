using System.Security.Claims;
using Gym.Application.Contracts.Tenants;

namespace Gym.Application.Tenants.Context;

/// <summary>
/// Resolves the authenticated tenant scope for the current principal.
/// No caller-supplied tenant identifier is accepted.
/// </summary>
public interface ITenantContextResolver
{
    TenantContext Resolve(ClaimsPrincipal principal);
}
