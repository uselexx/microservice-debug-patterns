using backend.Models;

namespace backend.Repositories;

/// <summary>
/// Dashboard-specific repository interface
/// </summary>
public interface IDashboardRepository : IRepository<Dashboard>
{
    Task<Dashboard?> GetByIdWithWidgetsAsync(int id);
    Task<IEnumerable<Dashboard>> GetAllWithWidgetsAsync();
}
