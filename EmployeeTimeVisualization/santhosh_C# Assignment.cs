using EmployeeTimeVisualization.Services;
using EmployeeTimeVisualization.DataAccess;
using EmployeeTimeVisualization.Presentation;

try
{
    var apiUrl = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
    if (string.IsNullOrWhiteSpace(apiUrl))
    {
        Console.WriteLine("Set the TIME_ENTRIES_API_URL environment variable first.");
        return;
    }

    using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
    var repository = new ApiTimeEntryRepository(httpClient, apiUrl);
    var service = new EmployeeReportService();

    Console.WriteLine("Fetching employee time entries...");
    var entries = await repository.GetTimeEntriesAsync();
    var summaries = service.CreateSummaries(entries);

    if (summaries.Count == 0)
    {
        Console.WriteLine("No valid employee time entries were found.");
        return;
    }

    var output = Path.Combine(AppContext.BaseDirectory, "Output");
    Directory.CreateDirectory(output);

    var htmlPath = Path.Combine(output, "employee_report.html");
    var pngPath = Path.Combine(output, "employee_hours_chart.png");

    new HtmlReportGenerator().Generate(summaries, htmlPath);
    new PieChartGenerator().Generate(summaries, pngPath);

    Console.WriteLine("\nEmployee totals:");
    foreach (var employee in summaries)
        Console.WriteLine($"{employee.EmployeeName}: {employee.TotalHours:F2} hours");

    Console.WriteLine($"\nHTML report: {htmlPath}");
    Console.WriteLine($"Pie chart:   {pngPath}");
}
catch (HttpRequestException ex)
{
    Console.Error.WriteLine($"API request failed: {ex.Message}");
    Environment.ExitCode = 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Application error: {ex.Message}");
    Environment.ExitCode = 1;
}
