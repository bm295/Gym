using Gym.Domain;

namespace Gym.Application.Repositories;

public interface IPaymentRepository
{
    Payment? GetById(Guid tenantId, Guid paymentId);
    IReadOnlyList<Payment> List(Guid tenantId);
    void Add(Payment payment);
}
