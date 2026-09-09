namespace Gym.Domain;

public sealed class Payment
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid BranchId { get; init; }
    public bool IsVoided { get; private set; }
    public string? VoidReason { get; private set; }

    public bool Void(string reason)
    {
        if (IsVoided)
        {
            return false;
        }

        IsVoided = true;
        VoidReason = reason;
        return true;
    }
}
