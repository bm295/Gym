namespace Gym.Application.Contracts.Reports;

public sealed record AttendanceReport(
    DateOnly StartDate,
    DateOnly EndDate,
    string TimeZoneId,
    int TotalCheckIns,
    double MeanDailyCheckIns,
    double MedianDailyCheckIns,
    int ModeDailyCheckIns,
    double DailyStandardDeviation,
    IReadOnlyList<int> PeakHours,
    IReadOnlyList<DailyAttendance> Days,
    IReadOnlyList<HourlyAttendance> Hours);

public sealed record DailyAttendance(DateOnly Date, int CheckIns);
public sealed record HourlyAttendance(int Hour, int CheckIns);
