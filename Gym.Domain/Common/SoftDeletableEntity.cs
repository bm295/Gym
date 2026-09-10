namespace Gym.Domain;

public abstract class SoftDeletableEntity : AuditableEntity
{
    public bool IsDeleted { get; private set; }

    public void MarkDeleted(DateTimeOffset updatedAt)
    {
        IsDeleted = true;
        Touch(updatedAt);
    }

    public void Restore(DateTimeOffset updatedAt)
    {
        IsDeleted = false;
        Touch(updatedAt);
    }
}
