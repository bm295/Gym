using Gym.Application.Contracts.Reports;

namespace Gym.Application.Reports;

/// <summary>Describes observed attendance, including zero-visit calendar days. Does not predict future attendance.</summary>
public static class AttendanceReportCalculator
{
    public static AttendanceReport Calculate(
        IEnumerable<DateTimeOffset> timestamps, DateOnly startDate, DateOnly endDate, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timestamps);
        ArgumentNullException.ThrowIfNull(timeZone);
        var dayCount = endDate.DayNumber - startDate.DayNumber + 1;
        if (dayCount is < 1 or > 366)
            throw new ArgumentOutOfRangeException(nameof(endDate), "Select between 1 and 366 calendar days.");

        var counts = new int[dayCount];
        var hours = new int[24];
        foreach (var timestamp in timestamps)
        {
            var local = TimeZoneInfo.ConvertTime(timestamp, timeZone);
            var index = DateOnly.FromDateTime(local.DateTime).DayNumber - startDate.DayNumber;
            if (index < 0 || index >= dayCount) continue;
            counts[index]++;
            hours[local.Hour]++;
        }

        var total = counts.Sum();
        var mean = (double)total / dayCount;
        var sorted = counts.Order().ToArray();
        var median = dayCount % 2 == 1 ? sorted[dayCount / 2]
            : ((double)sorted[dayCount / 2 - 1] + sorted[dayCount / 2]) / 2;
        // Match the original algorithm: the smallest daily count wins ties.
        var mode = sorted.GroupBy(count => count).OrderByDescending(group => group.Count()).ThenBy(group => group.Key).First().Key;
        var deviation = Math.Sqrt(counts.Sum(count => Math.Pow(count - mean, 2)) / dayCount);
        var peakCount = hours.Max();
        return new(startDate, endDate, timeZone.Id, total, mean, median, mode, deviation,
            peakCount == 0 ? [] : Enumerable.Range(0, 24).Where(hour => hours[hour] == peakCount).ToArray(),
            counts.Select((count, index) => new DailyAttendance(startDate.AddDays(index), count)).ToArray(),
            hours.Select((count, hour) => new HourlyAttendance(hour, count)).ToArray());
    }
}
