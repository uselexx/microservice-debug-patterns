using InventoryService.Models.Entities;
using Microsoft.EntityFrameworkCore;

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
}
