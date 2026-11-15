using Gym.Application.Reports;
using Xunit;

namespace Gym.Tests;

public class MembershipAnalyticsCalculatorTests
{
    [Fact]
    public void OverlappingRangesProduceCorrectDailyAndAverageCounts()
    {
        var start = new DateOnly(2026, 11, 1);
        var result = MembershipAnalyticsCalculator.Calculate(start, start.AddDays(4),
            new[] { (start, start.AddDays(2)), (start.AddDays(1), start.AddDays(3)) });

        Assert.Equal(1.2, result.AverageActiveSubscriptions, 10);
        Assert.Equal(6, result.TotalSubscriptionDays);
        Assert.Equal(new[] { 1, 2, 2, 1, 0 },
            result.DailyActiveSubscriptions.Select(day => day.Count));
    }

    [Fact]
    public void ClampsRangesToReportWindow()
    {
        var start = new DateOnly(2026, 11, 1);
        var result = MembershipAnalyticsCalculator.Calculate(start, start.AddDays(2),
            new[] { (start.AddDays(-10), start), (start.AddDays(2), start.AddDays(10)) });
        Assert.Equal(new[] { 1, 0, 1 }, result.DailyActiveSubscriptions.Select(day => day.Count));
    }

    [Fact]
    public void EmptyRangesIncludeZeroDays()
    {
        var start = new DateOnly(2026, 11, 1);
        var result = MembershipAnalyticsCalculator.Calculate(start, start.AddDays(1),
            Array.Empty<(DateOnly, DateOnly)>());
        Assert.Equal(0, result.AverageActiveSubscriptions);
        Assert.All(result.DailyActiveSubscriptions, day => Assert.Equal(0, day.Count));
    }

    [Fact]
    public void RejectsInvalidDateWindowsAndRanges()
    {
        var start = new DateOnly(2026, 11, 1);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            MembershipAnalyticsCalculator.Calculate(start, start.AddDays(-1),
                Array.Empty<(DateOnly, DateOnly)>()));
        Assert.Throws<ArgumentException>(() =>
            MembershipAnalyticsCalculator.Calculate(start, start,
                new[] { (start.AddDays(1), start) }));
    }
}
