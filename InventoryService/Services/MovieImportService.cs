using InventoryService.Models.Entities;
using InventoryService.Repositories;

namespace InventoryService.Services;

public sealed class MovieImportService(
    IMovieRepository repository,
    IUnitOfWork unitOfWork)
{
    public async Task ImportBatchAsync(
        IReadOnlyCollection<MovieEntity> movies,
        CancellationToken cancellationToken)
    {
        // Load Existing movies in BULK
        if (movies.Count == 0)
            return;

        var legacyIds = movies
            .Select(x => x.LegacyId)
            .ToArray();

        var existing = await repository.GetByLegacyIdsAsync(
            legacyIds,
            cancellationToken);

        //var dedupedMovies = movies
        //    .GroupBy(x => x.ImdbId ?? $"legacy:{x.LegacyId}")
        //    .Select(g => g.Last())
        //    .ToList();
        var existingMap = existing
            .ToDictionary(x => x.LegacyId);

        // Decide insert vs update
        var toInsert = new List<MovieEntity>();
        var toUpdate = new List<MovieEntity>();

        foreach (var movie in movies)
        {
            if (existingMap.TryGetValue(movie.LegacyId, out var found))
            {
                movie.Id = found.Id;
                toUpdate.Add(movie);
            }
            else
            {
                toInsert.Add(movie);
            }
        }

        if (toInsert.Count > 0)
            await repository.AddRangeAsync(toInsert, cancellationToken);

        if (toUpdate.Count > 0)
            repository.UpdateRange(toUpdate);

        // save once per batch
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}