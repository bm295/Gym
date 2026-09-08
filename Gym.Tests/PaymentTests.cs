using Gym.Domain;

namespace Gym.Tests;

public sealed class PaymentTests
{
    [Fact]
    public void Void_marks_a_payment_as_voided_with_the_reason()
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = "tenant-1",
            BranchId = "branch-1"
        };

        var voided = payment.Void("Duplicate payment");

        Assert.True(voided);
        Assert.True(payment.IsVoided);
        Assert.Equal("Duplicate payment", payment.VoidReason);
    }

    [Fact]
    public void Void_does_not_change_an_already_voided_payment()
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = "tenant-1",
            BranchId = "branch-1"
        };

        payment.Void("Original reason");

        var voidedAgain = payment.Void("Replacement reason");

        Assert.False(voidedAgain);
        Assert.Equal("Original reason", payment.VoidReason);
    }
}
