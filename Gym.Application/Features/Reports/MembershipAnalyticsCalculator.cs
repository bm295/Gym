using Gym.Application.Contracts.Reports;

namespace Gym.Application.Reports;

/// <summary>Counts subscription coverage by calendar day using a difference array.
/// Input ranges must already be filtered for tenant, branch and applicable subscription status.
/// End dates are inclusive; paused or cancelled periods must be excluded by the caller.
/// </summary>
public static class MembershipAnalyticsCalculator
{
    public static MembershipUtilizationReport Calculate(
        DateOnly startDate,
        DateOnly endDate,
        IEnumerable<(DateOnly Start, DateOnly End)> ranges)
    {
        ArgumentNullException.ThrowIfNull(ranges);
        var dayCount = endDate.DayNumber - startDate.DayNumber + 1;
        if (dayCount is < 1 or > 366)
            throw new ArgumentOutOfRangeException(nameof(endDate), "Select between 1 and 366 calendar days.");

        var differences = new long[dayCount + 1];
        long totalSubscriptionDays = 0;
        foreach (var (start, end) in ranges)
        {
            if (end < start)
                throw new ArgumentException("Subscription end date precedes start date.", nameof(ranges));
            var from = Math.Max(start.DayNumber, startDate.DayNumber);
            var to = Math.Min(end.DayNumber, endDate.DayNumber);
            if (from > to) continue;

            differences[from - startDate.DayNumber]++;
            differences[to - startDate.DayNumber + 1]--;
            totalSubscriptionDays += (long)to - from + 1;
        }

        var daily = new List<DailyActiveSubscriptions>(dayCount);
        long running = 0;
        for (var i = 0; i < dayCount; i++)
        {
            running += differences[i];
            daily.Add(new DailyActiveSubscriptions(startDate.AddDays(i), checked((int)running)));
        }

        return new MembershipUtilizationReport(startDate, endDate,
            (double)totalSubscriptionDays / dayCount, totalSubscriptionDays, daily);
    }
}
