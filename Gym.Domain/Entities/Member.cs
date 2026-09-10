namespace Gym.Domain;

public sealed class Member : SoftDeletableEntity
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string MemberCode { get; init; }
    public required string FullName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string NormalizedPhoneNumber { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public string? Gender { get; init; }
    public string? Email { get; init; }
    public string? Address { get; init; }
    public Guid? HomeBranchId { get; init; }
    public required MemberStatus Status { get; init; }
    public string? Note { get; init; }
}
