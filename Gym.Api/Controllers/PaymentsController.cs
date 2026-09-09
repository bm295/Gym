using Gym.Api.Authorization;
using Gym.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gym.Api.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.CanVoidPayment)]
[Route("api/tenants/{tenantId:guid}/branches/{branchId:guid}/payments")]
public sealed class PaymentsController(IPaymentService payments, ILogger<PaymentsController> logger)
    : ControllerBase
{
    [HttpPost("{paymentId:guid}/void")]
    public IActionResult Void(Guid tenantId, Guid branchId, Guid paymentId, VoidPaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            ModelState.AddModelError(nameof(request.Reason), "A reason is required to void a payment.");
            return ValidationProblem(ModelState);
        }

        var result = payments.Void(tenantId, branchId, paymentId, request.Reason.Trim());
        if (result == VoidPaymentResult.Success)
        {
            logger.LogInformation(
                "Payment {PaymentId} was voided for tenant {TenantId} and branch {BranchId}",
                paymentId,
                tenantId,
                branchId);
        }

        return result switch
        {
            VoidPaymentResult.Success => NoContent(),
            VoidPaymentResult.NotFound => NotFound(),
            VoidPaymentResult.AlreadyVoided => Conflict(new { message = "Payment has already been voided." }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}

public sealed record VoidPaymentRequest(string Reason);
