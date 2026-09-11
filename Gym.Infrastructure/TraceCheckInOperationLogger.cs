using System.Diagnostics;
using Gym.Application.CheckIns;
using Gym.Domain;

namespace Gym.Infrastructure;

public sealed class TraceCheckInOperationLogger : ICheckInOperationLogger
{
    public void LogSuccessfulCheckIn(CheckIn checkIn)
    {
        ArgumentNullException.ThrowIfNull(checkIn);

        Trace.TraceInformation(
            "Member check-in {0} succeeded for tenant {1}, member {2}, subscription {3}, and branch {4} at {5:O}.",
            checkIn.Id,
            checkIn.TenantId,
            checkIn.MemberId,
            checkIn.SubscriptionId,
            checkIn.BranchId,
            checkIn.CheckedInAt);
    }
}
