using Gym.Application;
using Gym.Domain;

namespace Gym.Infrastructure;

public sealed class PaymentService : IPaymentService
{
    private readonly object _gate = new();
    private readonly List<Payment> _payments = [];

    public VoidPaymentResult Void(string tenantId, string branchId, Guid paymentId, string reason)
    {
        lock (_gate)
        {
            var payment = _payments.SingleOrDefault(payment =>
                payment.Id == paymentId
                && payment.TenantId == tenantId
                && payment.BranchId == branchId);

            if (payment is null)
            {
                return VoidPaymentResult.NotFound;
            }

            return payment.Void(reason)
                ? VoidPaymentResult.Success
                : VoidPaymentResult.AlreadyVoided;
        }
    }
}
