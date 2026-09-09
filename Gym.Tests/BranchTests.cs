using Gym.Domain;

namespace Gym.Tests;

public sealed class BranchTests
{
    [Fact]
    public void MarkDeleted_and_restore_update_the_deletion_state_and_timestamp()
    {
        var createdAt = new DateTimeOffset(2026, 9, 9, 8, 0, 0, TimeSpan.Zero);
        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            BranchCode = "HCM-01",
            Name = "District 1",
            Status = "Active",
            CreatedAt = createdAt,
            UpdatedAt = createdAt
        };
        var deletedAt = createdAt.AddHours(1);
        var restoredAt = deletedAt.AddHours(1);

        branch.MarkDeleted(deletedAt);

        Assert.True(branch.IsDeleted);
        Assert.Equal(deletedAt, branch.UpdatedAt);

        branch.Restore(restoredAt);

        Assert.False(branch.IsDeleted);
        Assert.Equal(restoredAt, branch.UpdatedAt);
    }
}
