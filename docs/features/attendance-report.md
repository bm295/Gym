# Attendance habits report

Open `/prototype/reports` and choose a branch in the application header. The report uses check-ins from the current prototype session, replacing the former static report cards. It does not fetch server attendance. The default period covers the last 30 completed calendar days in Vietnam time (`Asia/Ho_Chi_Minh`). Apply a custom inclusive period of 1–366 days to include other dates, including today.

## Metrics

- Total check-ins counts visits, including multiple visits by the same member.
- Daily mean divides visits by every calendar day in the period.
- Daily median sorts the daily counts and averages the middle two for an even number of days.
- Daily mode is the most frequent daily count; the smallest count wins ties.
- Daily standard deviation uses the population formula, dividing squared differences from the mean by the number of days.
- Busiest arrival hours are the one-hour local buckets with the most check-ins. All ties are shown. No visits means no peak.

Daily and hourly tables make the underlying pattern visible. Days without recorded visits count as zero, including closed days; there is no opening-calendar model yet. A custom range containing today includes its partial count, and future days count as zero. These are descriptive metrics, not attendance forecasts or concurrent occupancy estimates. No confidence interval is presented.

## Implementation boundary

`AttendanceReportCalculator` in Application shares calculations between the prototype page and `ITenantReportQuery.GetAttendanceReport`. The in-memory query filters both tenant and branch before calculating. This query is not an HTTP endpoint; any future API caller must authorize the requested tenant and branch using the existing access policies. The prototype page only reads its session state and responds to branch and check-in changes.

Tests cover mean/median/mode/deviation, empty periods, zero days, tied peaks, UTC-to-local date boundaries, invalid ranges, tenant/branch isolation, and UI period and branch changes.
