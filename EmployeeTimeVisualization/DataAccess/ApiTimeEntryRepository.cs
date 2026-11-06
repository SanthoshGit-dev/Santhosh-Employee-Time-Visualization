using System.Text.Json;
using EmployeeTimeVisualization.Services.Interfaces;
using EmployeeTimeVisualization.Models;

namespace EmployeeTimeVisualization.DataAccess;

public sealed class ApiTimeEntryRepository : ITimeEntryRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public ApiTimeEntryRepository(HttpClient httpClient, string apiUrl)
    {
        _httpClient = httpClient;
        _apiUrl = apiUrl;
    }

    public async Task<IReadOnlyList<TimeEntry>> GetTimeEntriesAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(_apiUrl, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var entries = await JsonSerializer.DeserializeAsync<List<TimeEntry>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        return entries ?? new List<TimeEntry>();
    }
}
