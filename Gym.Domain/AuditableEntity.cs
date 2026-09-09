namespace Gym.Domain;

public abstract class AuditableEntity
{
    private DateTimeOffset _updatedAt;

    public required DateTimeOffset CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }

    public required DateTimeOffset UpdatedAt
    {
        get => _updatedAt;
        init => _updatedAt = value;
    }
    public Guid? UpdatedBy { get; init; }

    protected void Touch(DateTimeOffset updatedAt) => _updatedAt = updatedAt;
}
