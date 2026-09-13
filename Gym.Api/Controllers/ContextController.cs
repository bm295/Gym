using Gym.Application.Contracts.Tenants;
using Gym.Application.Tenants.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/context")]
public sealed class ContextController(ITenantContextResolver tenantContextResolver) : ControllerBase
{
    [HttpGet]
    public ActionResult<TenantContext> Get()
    {
        return Ok(tenantContextResolver.Resolve(User));
    }
}
