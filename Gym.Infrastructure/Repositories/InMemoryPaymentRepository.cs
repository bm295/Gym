using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure.Repositories;

public sealed class InMemoryPaymentRepository(InMemoryGymDataStore store) : IPaymentRepository
{
    public Payment? GetById(Guid tenantId, Guid paymentId) =>
        store.GetPayments(tenantId).SingleOrDefault(payment => payment.Id == paymentId);

    public IReadOnlyList<Payment> List(Guid tenantId) => store.GetPayments(tenantId);

    public void Add(Payment payment) => store.Add(payment);
}
