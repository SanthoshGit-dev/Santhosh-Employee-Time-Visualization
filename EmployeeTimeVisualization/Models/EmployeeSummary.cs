namespace EmployeeTimeVisualization.Models;
public sealed class EmployeeSummary
{
    public string EmployeeName { get; set; }
    public double TotalHours { get; set; }
    public double PercentageOfTotal { get; set; }
    public EmployeeSummary(string employeeName, double totalHours)
    {
        EmployeeName = employeeName;
        TotalHours = totalHours;
    }
}
