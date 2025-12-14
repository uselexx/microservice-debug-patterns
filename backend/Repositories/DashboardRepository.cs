using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

/// <summary>
/// Dashboard-specific repository implementation with eager loading support
/// </summary>
public class DashboardRepository : Repository<Dashboard>, IDashboardRepository
{
    private readonly PGContext _context;

    public DashboardRepository(PGContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Dashboard?> GetByIdWithWidgetsAsync(int id)
    {
        return await _context.Dashboards
            .Include(d => d.Widgets)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<Dashboard>> GetAllWithWidgetsAsync()
    {
        return await _context.Dashboards
            .Include(d => d.Widgets)
            .ToListAsync();
    }
}
