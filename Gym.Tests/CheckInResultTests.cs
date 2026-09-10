using Gym.Application.CheckIns;

namespace Gym.Tests;

public sealed class CheckInResultTests
{
    [Fact]
    public void Success_includes_the_created_check_in_id()
    {
        var checkInId = Guid.NewGuid();

        var result = CheckInResult.Success(checkInId);

        Assert.True(result.IsSuccess);
        Assert.Equal(checkInId, result.CheckInId);
        Assert.Null(result.RejectionReason);
    }

    [Fact]
    public void Rejected_result_includes_a_reason_and_no_check_in_id()
    {
        var result = CheckInResult.Rejected(CheckInRejectionReason.MemberInactive);

        Assert.False(result.IsSuccess);
        Assert.Null(result.CheckInId);
        Assert.Equal(CheckInRejectionReason.MemberInactive, result.RejectionReason);
    }
}
