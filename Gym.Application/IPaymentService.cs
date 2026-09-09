namespace Gym.Application;

public interface IPaymentService
{
    VoidPaymentResult Void(Guid tenantId, Guid branchId, Guid paymentId, string reason);
}

public enum VoidPaymentResult
{
    Success,
    NotFound,
    AlreadyVoided
}
