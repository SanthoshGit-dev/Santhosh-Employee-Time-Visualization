using EmployeeTimeVisualization.Models;
namespace EmployeeTimeVisualization.Services;

public sealed class EmployeeReportService
{
    public List<EmployeeSummary> CreateSummaries(IEnumerable<TimeEntry> entries)
    {
        // Ignore missing employee names and invalid/zero-length time intervals.
        var summaries = entries
            .Where(e => !string.IsNullOrWhiteSpace(e.EmployeeName)
                        && e.EndTimeUtc > e.StarTimeUtc)
            .GroupBy(e => e.EmployeeName!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new EmployeeSummary(g.Key,
                Math.Round(g.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).TotalHours), 2)))
            .OrderByDescending(e => e.TotalHours)
            .ThenBy(e => e.EmployeeName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var total = summaries.Sum(e => e.TotalHours);
        foreach (var employee in summaries)
            employee.PercentageOfTotal = total > 0 ? employee.TotalHours / total * 100 : 0;

        return summaries;
    }
}
