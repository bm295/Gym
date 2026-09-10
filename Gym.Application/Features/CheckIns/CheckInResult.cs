namespace Gym.Application.CheckIns;

public sealed record CheckInResult
{
    private CheckInResult(bool isSuccess, Guid? checkInId, CheckInRejectionReason? rejectionReason)
    {
        IsSuccess = isSuccess;
        CheckInId = checkInId;
        RejectionReason = rejectionReason;
    }

    public bool IsSuccess { get; }

    public Guid? CheckInId { get; }

    public CheckInRejectionReason? RejectionReason { get; }

    public static CheckInResult Success(Guid checkInId) => new(true, checkInId, null);

    public static CheckInResult Rejected(CheckInRejectionReason rejectionReason) => new(false, null, rejectionReason);
}
