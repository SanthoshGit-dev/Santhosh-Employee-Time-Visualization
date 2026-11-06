using System.Net;
using System.Text;
using EmployeeTimeVisualization.Models;

namespace EmployeeTimeVisualization.Presentation;

public sealed class HtmlReportGenerator
{
    public void Generate(IReadOnlyList<EmployeeSummary> employees, string outputPath)
    {
        var html = new StringBuilder();
        html.AppendLine("<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\">");
        html.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        html.AppendLine("<title>Employee Time Report</title><style>");
        html.AppendLine("body{font-family:Arial,sans-serif;margin:36px;color:#222}table{border-collapse:collapse;width:100%;max-width:900px}");
        html.AppendLine("th,td{border:1px solid #ddd;padding:12px;text-align:left}th{background:#26364a;color:white}");
        html.AppendLine("tr:nth-child(even){background:#f4f6f8}.low-hours{background:#ffe2e2!important;color:#8b0000;font-weight:bold}");
        html.AppendLine(".number{text-align:right}</style></head><body>");
        html.AppendLine("<h1>Employee Time Report</h1>");
        html.AppendLine("<p>Employees are sorted by total working hours, highest to lowest. Rows below 100 hours are highlighted.</p>");
        html.AppendLine("<table><thead><tr><th>Employee Name</th><th class=\"number\">Total Hours Worked</th><th class=\"number\">Share of Total</th></tr></thead><tbody>");

        foreach (var employee in employees)
        {
            var css = employee.TotalHours < 100 ? " class=\"low-hours\"" : "";
            html.Append("<tr").Append(css).Append("><td>")
                .Append(WebUtility.HtmlEncode(employee.EmployeeName)).Append("</td><td class=\"number\">")
                .Append(employee.TotalHours.ToString("N2", System.Globalization.CultureInfo.InvariantCulture))
                .Append("</td><td class=\"number\">")
                .Append(employee.PercentageOfTotal.ToString("N2", System.Globalization.CultureInfo.InvariantCulture))
                .AppendLine("%</td></tr>");
        }

        html.AppendLine("</tbody></table></body></html>");
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
        File.WriteAllText(outputPath, html.ToString(), Encoding.UTF8);
    }
}
