namespace Gym.Application;

public interface IPaymentService
{
    VoidPaymentResult Void(string tenantId, string branchId, Guid paymentId, string reason);
}

public enum VoidPaymentResult
{
    Success,
    NotFound,
    AlreadyVoided
}
