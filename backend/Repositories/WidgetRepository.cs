using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

/// <summary>
/// Widget-specific repository implementation
/// </summary>
public class WidgetRepository : Repository<Widget>, IWidgetRepository
{
    private readonly PGContext _context;

    public WidgetRepository(PGContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Widget>> GetByDashboardIdAsync(int dashboardId)
    {
        return await _context.Widgets
            .Where(w => w.DashboardId == dashboardId)
            .OrderBy(w => w.DisplayOrder)
            .ToListAsync();
    }
}
