using Gym.Application;
using Gym.Application.CheckIns;
using Gym.Application.Repositories;
using Gym.Domain;

namespace Gym.Infrastructure;

public sealed class CheckInService(
    IMemberRepository members,
    IBranchRepository branches,
    ISubscriptionRepository subscriptions,
    ICheckInRepository checkIns,
    IUtcClock clock,
    ICheckInOperationLogger operationLogger) : ICheckInService
{
    private static readonly TimeSpan DuplicateWindow = TimeSpan.FromMinutes(5);

    public CheckInResult CheckIn(CheckInRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var lookup = request.MemberCodeOrNormalizedPhone?.Trim();
        if (string.IsNullOrEmpty(lookup))
        {
            return CheckInResult.Rejected(CheckInRejectionReason.MemberNotFound);
        }

        var member = members.FindByCodeOrNormalizedPhone(request.TenantId, lookup);
        if (member is null)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.MemberNotFound);
        }

        if (member.Status != MemberStatus.Active)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.MemberInactive);
        }

        // Resolve the branch through the tenant-scoped repository. Treat a branch
        // outside the tenant exactly like any other branch the subscription cannot
        // use, without disclosing whether that branch exists.
        if (branches.GetById(request.TenantId, request.BranchId) is null)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.BranchNotPermitted);
        }

        var subscription = subscriptions.GetByMember(request.TenantId, member.Id)
            .FirstOrDefault(candidate => candidate.Status == SubscriptionStatus.Active);
        if (subscription is null)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.NoActiveSubscription);
        }

        var now = clock.UtcNow.ToUniversalTime();
        var currentDate = DateOnly.FromDateTime(now.UtcDateTime);
        if (currentDate < subscription.StartDate)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.SubscriptionNotStarted);
        }

        if (currentDate > subscription.EndDate)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.SubscriptionExpired);
        }

        if (subscription.HomeBranchId != request.BranchId && !subscription.AllowCrossBranchSnapshot)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.BranchNotPermitted);
        }

        if (subscription.RemainingVisits == 0)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.NoVisitsRemaining);
        }

        if (checkIns.GetByMemberAndBranchSince(
                request.TenantId,
                member.Id,
                request.BranchId,
                now.Subtract(DuplicateWindow)).Count > 0)
        {
            return CheckInResult.Rejected(CheckInRejectionReason.DuplicateCheckIn);
        }

        var checkIn = new CheckIn
        {
            Id = Guid.NewGuid(),
            TenantId = request.TenantId,
            MemberId = member.Id,
            SubscriptionId = subscription.Id,
            BranchId = request.BranchId,
            CheckedInAt = now,
            CreatedBy = request.StaffUserId,
            Note = request.Note
        };
        checkIns.Add(checkIn);
        operationLogger.LogSuccessfulCheckIn(checkIn);

        return CheckInResult.Success(checkIn.Id);
    }
}
