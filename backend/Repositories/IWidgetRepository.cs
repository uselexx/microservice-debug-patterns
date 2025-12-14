using backend.Models;

namespace backend.Repositories;

/// <summary>
/// Widget-specific repository interface
/// </summary>
public interface IWidgetRepository : IRepository<Widget>
{
    Task<IEnumerable<Widget>> GetByDashboardIdAsync(int dashboardId);
}
