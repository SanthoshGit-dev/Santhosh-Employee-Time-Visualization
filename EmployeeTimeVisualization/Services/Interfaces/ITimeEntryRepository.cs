using EmployeeTimeVisualization.Models;

namespace EmployeeTimeVisualization.Services.Interfaces;

public interface ITimeEntryRepository
{
    Task<IReadOnlyList<TimeEntry>> GetTimeEntriesAsync(
        CancellationToken cancellationToken = default);
}
