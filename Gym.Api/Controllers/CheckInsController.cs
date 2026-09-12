using System.Security.Claims;
using Gym.Api.Authorization;
using Gym.Application.CheckIns;
using Gym.Application.Contracts.CheckIns;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.CanCheckIn)]
[Route("api/tenants/{tenantId:guid}/branches/{branchId:guid}/check-ins")]
public sealed class CheckInsController(ICheckInService checkIns) : ControllerBase
{
    [HttpPost]
    public ActionResult<CheckInApiResponse> CheckIn(
        Guid tenantId,
        Guid branchId,
        CheckInApiRequest request)
    {
        var result = checkIns.CheckIn(new CheckInRequest(
            tenantId,
            branchId,
            request.MemberCodeOrNormalizedPhone,
            GetStaffUserId(),
            request.Note));

        return Ok(new CheckInApiResponse(
            result.IsSuccess,
            result.CheckInId,
            result.RejectionReason));
    }

    private Guid? GetStaffUserId()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(value, out var staffUserId) ? staffUserId : null;
    }
}
