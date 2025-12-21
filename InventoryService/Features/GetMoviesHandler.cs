using InventoryService.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace InventoryService.Features;

public class GetMoviesHandler(IMovieRepository repository, ILogger<GetMoviesHandler> logger) : IConsumer<GetMoviesRequest>
{
    public async Task Consume(ConsumeContext<GetMoviesRequest> context)
    {
        logger.LogInformation("Movies Handler received request for cursor {} pagesize {}", context.Message.cursor, context.Message.pageSize);
        // 1. Fetch the entities from the repository
        var result = await repository.GetPagedAsync(
            context.Message.cursor,
            context.Message.pageSize,
            context.CancellationToken);

        // 2. Map the List<MovieEntity> to List<GetMovieResponse>
        var mappedMovies = result.Data.Select(m => new GetMovieResponse(
            CorrelationId: context.CorrelationId ?? Guid.NewGuid(),
            Movie: new MovieDto(
                m.Id,
                m.LegacyId,
                m.Title ?? "Unknown Title",
                m.Overview,
                m.ReleaseDate ?? DateTime.MinValue,
                m.Popularity
            ),
            Error: null
        )).ToList();

        // 3. Wrap the mapped list back into a PagedResponse container
        var pagedResponse = new PagedResponse<GetMovieResponse>(
            mappedMovies,
            result.NextCursor,
            result.HasMore
        );

        // 4. Respond with the final DTO contract
        await context.RespondAsync(new GetMoviesResponse(pagedResponse));
    }
}
