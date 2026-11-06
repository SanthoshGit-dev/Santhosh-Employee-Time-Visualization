namespace EmployeeTimeVisualization.Models;
public sealed class TimeEntry
{
    public string? EmployeeName { get; set; }
    public DateTime StarTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
}
