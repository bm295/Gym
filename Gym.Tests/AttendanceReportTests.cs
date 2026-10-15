using Gym.Application.Reports;
using Gym.Infrastructure.Repositories;
using Gym.Domain;

namespace Gym.Tests;

public sealed class AttendanceReportTests
{
    private static readonly TimeZoneInfo Vietnam = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    private static readonly DateOnly Start = new(2026, 10, 1);

    [Fact]
    public void Calculates_original_statistics_including_zero_days_and_tied_peak_hours()
    {
        // Daily counts: 0, 2, 2, 4. Hourly peaks: 06 and 18, four visits each.
        var visits = new[] { Local(2, 6), Local(2, 18), Local(3, 6), Local(3, 18),
            Local(4, 6), Local(4, 6), Local(4, 18), Local(4, 18) };
        var report = AttendanceReportCalculator.Calculate(visits, Start, Start.AddDays(3), Vietnam);
        Assert.Equal(8, report.TotalCheckIns);
        Assert.Equal(2, report.MeanDailyCheckIns);
        Assert.Equal(2, report.MedianDailyCheckIns);
        Assert.Equal(2, report.ModeDailyCheckIns);
        Assert.Equal(Math.Sqrt(2), report.DailyStandardDeviation, 10);
        Assert.Equal(new[] { 0, 2, 2, 4 }, report.Days.Select(day => day.CheckIns));
        Assert.Equal(new[] { 6, 18 }, report.PeakHours);
        Assert.Equal(24, report.Hours.Count);
    }

    [Fact]
    public void Even_median_preserves_fractional_value()
    {
        var report = AttendanceReportCalculator.Calculate(new[] { Local(2, 8) }, Start, Start.AddDays(1), Vietnam);
        Assert.Equal(0.5, report.MeanDailyCheckIns);
        Assert.Equal(0.5, report.MedianDailyCheckIns);
        Assert.Equal(0.5, report.DailyStandardDeviation);
        Assert.Equal(0, report.ModeDailyCheckIns);
    }

    [Fact]
    public void Converts_utc_to_local_calendar_boundaries_and_excludes_outside_visits()
    {
        var report = AttendanceReportCalculator.Calculate(new[] {
            new DateTimeOffset(2026, 9, 30, 16, 59, 59, TimeSpan.Zero),
            new DateTimeOffset(2026, 9, 30, 17, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 10, 1, 16, 59, 59, TimeSpan.Zero),
            new DateTimeOffset(2026, 10, 1, 17, 0, 0, TimeSpan.Zero)
        }, Start, Start, Vietnam);
        Assert.Equal(2, report.TotalCheckIns);
        Assert.Equal(new[] { 0, 23 }, report.PeakHours);
        Assert.Equal(0, report.DailyStandardDeviation);
    }

    [Fact]
    public void Empty_period_has_zero_statistics_and_no_peak()
    {
        var report = AttendanceReportCalculator.Calculate([], Start, Start.AddDays(2), Vietnam);
        Assert.Equal(3, report.Days.Count);
        Assert.Equal(0, report.TotalCheckIns);
        Assert.Equal(0, report.MeanDailyCheckIns);
        Assert.Equal(0, report.MedianDailyCheckIns);
        Assert.Equal(0, report.ModeDailyCheckIns);
        Assert.Equal(0, report.DailyStandardDeviation);
        Assert.Empty(report.PeakHours);
    }

    [Fact]
    public void Odd_median_and_mode_tie_use_sorted_daily_counts()
    {
        var report = AttendanceReportCalculator.Calculate(new[] { Local(2, 8), Local(3, 8), Local(3, 9) }, Start, Start.AddDays(2), Vietnam);
        Assert.Equal(1, report.MedianDailyCheckIns);
        Assert.Equal(0, report.ModeDailyCheckIns);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public void Rejects_invalid_or_excessive_period(int endOffset) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => AttendanceReportCalculator.Calculate([], Start, Start.AddDays(endOffset), Vietnam));

    [Fact]
    public void Query_isolates_tenant_branch_and_date_range()
    {
        var store = new InMemoryGymDataStore();
        var tenant = Guid.NewGuid();
        var branch = Guid.NewGuid();
        foreach (var (tenantId, branchId, time) in new[] {
            (tenant, branch, Local(1, 8)), (Guid.NewGuid(), branch, Local(1, 8)),
            (tenant, Guid.NewGuid(), Local(1, 8)), (tenant, branch, Local(2, 8)) })
            store.Add(new CheckIn { Id = Guid.NewGuid(), TenantId = tenantId, BranchId = branchId,
                MemberId = Guid.NewGuid(), SubscriptionId = Guid.NewGuid(), CheckedInAt = time });
        var report = new InMemoryTenantReportQuery(store).GetAttendanceReport(tenant, branch, Start, Start, Vietnam);
        Assert.Equal(1, report.TotalCheckIns);
    }

    private static DateTimeOffset Local(int day, int hour) => new(2026, 10, day, hour, 0, 0, TimeSpan.FromHours(7));
}
