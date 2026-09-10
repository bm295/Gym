namespace Gym.Domain;

public sealed class StaffUser : AuditableEntity
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required string FullName { get; init; }
    public required string Role { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset? LastLoginAt { get; init; }
}
