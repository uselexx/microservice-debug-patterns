using InventoryService.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace InventoryService.Repositories;

public interface IMovieRepository
{
    Task<MovieEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MovieEntity?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MovieEntity>> GetAllAsync();

    Task AddAsync(MovieEntity movie, CancellationToken cancellationToken = default);
    void Update(MovieEntity movie);
    Task Delete(int id);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    // Batch access
    Task<IReadOnlyList<MovieEntity>> GetByLegacyIdsAsync(
        IReadOnlyCollection<int> legacyIds,
        CancellationToken cancellationToken);

    Task AddRangeAsync(
        IReadOnlyCollection<MovieEntity> movies,
        CancellationToken cancellationToken);

    void UpdateRange(IReadOnlyCollection<MovieEntity> movies);

    Task<MovieEntity?> FindAsync(
    string? title,
    string? description,
    int? legacyId,
    CancellationToken cancellationToken);

    Task<PagedResponse<MovieEntity>> GetPagedAsync(
        int? cursor,
        int pageSize,
        CancellationToken cancellationToken = default);

}

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _db;

    public MovieRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<MovieEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<MovieEntity?> GetByLegacyIdAsync(int legacyId, CancellationToken cancellationToken = default)
    {
        return await _db.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LegacyId == legacyId, cancellationToken);
    }

    public async Task<IReadOnlyList<MovieEntity>> GetAllAsync()
    {
        return await _db.Movies
            .AsNoTracking()
            .OrderByDescending(x => x.Popularity)
            .ToListAsync();
    }

    public async Task AddAsync(MovieEntity movie, CancellationToken cancellationToken = default)
    {
        await _db.Movies.AddAsync(movie, cancellationToken);
    }

    public void Update(MovieEntity movie)
    {
        _db.Movies.Update(movie);
    }

    public async Task Delete(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie is null)
            return;

        _db.Movies.Remove(movie);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Movies.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MovieEntity>> GetByLegacyIdsAsync(
    IReadOnlyCollection<int> legacyIds,
    CancellationToken cancellationToken)
    {
        return await _db.Movies
            .Where(x => legacyIds.Contains(x.LegacyId))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // ✅ HIGH-PERFORMANCE BULK INSERT
    public async Task AddRangeAsync(
        IReadOnlyCollection<MovieEntity> movies,
        CancellationToken cancellationToken)
    {
        if (movies.Count == 0)
            return;

        await _db.Movies.AddRangeAsync(movies, cancellationToken);
    }

    // ✅ HIGH-PERFORMANCE BULK UPDATE
    public void UpdateRange(IReadOnlyCollection<MovieEntity> movies)
    {
        if (movies.Count == 0)
            return;

        _db.Movies.UpdateRange(movies);
    }

    public async Task<MovieEntity?> FindAsync(string? title, string? description, int? legacyId, CancellationToken cancellationToken)
    {
        var query = _db.Movies.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(description))
            query = query.Where(x => x.Overview!.Contains(description));

        if (legacyId.HasValue)
            query = query.Where(x => x.LegacyId == legacyId);

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResponse<MovieEntity>> GetPagedAsync(
        int? cursor,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        // 1. Create the base query with NoTracking for performance
        var query = _db.Movies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .AsQueryable();

        // 2. Apply the cursor filter if it exists
        if (cursor.HasValue)
        {
            query = query.Where(x => x.Id > cursor.Value);
        }

        // 3. Fetch pageSize + 1 to determine if there's a next page
        var items = await query
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        // 4. Calculate metadata
        var hasMore = items.Count > pageSize;
        var data = items.Take(pageSize).ToList();

        // Use the ID of the last element in the 'data' list as the next cursor
        var nextCursor = hasMore ? data.Last().Id : (int?)null;

        return new PagedResponse<MovieEntity>(data, nextCursor, hasMore);
    }
}
