namespace Gym.Application.Contracts.Reports;

public sealed record DailyActiveSubscriptions(DateOnly Date, int Count);

public sealed record MembershipUtilizationReport(
    DateOnly StartDate,
    DateOnly EndDate,
    double AverageActiveSubscriptions,
    long TotalSubscriptionDays,
    IReadOnlyList<DailyActiveSubscriptions> DailyActiveSubscriptions);
