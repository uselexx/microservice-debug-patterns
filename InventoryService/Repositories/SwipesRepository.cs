using InventoryService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories;

public interface ISwipesRepository
{
    Task<IReadOnlyList<SwipeEntity>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task AddSwipeAsync(
        SwipeEntity swipe,
        CancellationToken cancellationToken = default);
}

public class SwipesRepository : ISwipesRepository
{
    private readonly ApplicationDbContext _db;

    public SwipesRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SwipeEntity>> GetByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Set<SwipeEntity>()
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddSwipeAsync(
        SwipeEntity swipe,
        CancellationToken cancellationToken = default)
    {
        await _db.Set<SwipeEntity>()
            .AddAsync(swipe, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }
}