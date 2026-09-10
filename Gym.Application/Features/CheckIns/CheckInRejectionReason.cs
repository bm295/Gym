namespace Gym.Application.CheckIns;

public enum CheckInRejectionReason
{
    MemberNotFound,
    MemberInactive,
    NoActiveSubscription,
    SubscriptionNotStarted,
    SubscriptionExpired,
    BranchNotPermitted,
    NoVisitsRemaining,
    DuplicateCheckIn
}
