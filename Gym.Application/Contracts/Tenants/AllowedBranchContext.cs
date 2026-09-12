namespace Gym.Application.Contracts.Tenants;

public sealed record AllowedBranchContext(
    Guid BranchId,
    string BranchCode,
    string BranchName);
