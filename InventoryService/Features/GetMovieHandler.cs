using InventoryService.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts;

namespace InventoryService.Features;

public class GetMovieHandler(ILogger<GetMovieHandler> logger, IMovieRepository movieRepository) : IConsumer<GetMovieRequest>
{
    public async Task Consume(ConsumeContext<GetMovieRequest> context)
    {
        logger.LogInformation("Received message {}", context.Message.Title);


        var request = context.Message;

        var movie = await movieRepository.FindAsync(
            request.Title,
            request.Description,
            request.LegacyId,
            context.CancellationToken);

        if (movie == null)
        {
            await context.RespondAsync(new GetMovieResponse(
                context.CorrelationId,
                null,
                "Movie not found"));
            return;
        }

        await context.RespondAsync(new GetMovieResponse(
            context.CorrelationId,
            new MovieDto(
                movie.Id,
                movie.LegacyId,
                movie.Title ?? "Unknown",
                movie.Overview,
                movie.ReleaseDate ?? DateTime.MinValue,
                movie.Popularity),
            null));
    }
}
