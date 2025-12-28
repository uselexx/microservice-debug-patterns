using InventoryService.Models.Entities;
using InventoryService.Repositories;
using MassTransit;
using Shared.Contracts;

namespace InventoryService.Features;

public class PostSwipeHandler(ISwipesRepository swipesRepository) : IConsumer<PostSwipeRequest>
{
    public async Task Consume(ConsumeContext<PostSwipeRequest> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var swipeEntity = new SwipeEntity
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            MovieId = request.Swipe.MovieId,
            IsLiked = request.Swipe.IsLiked,
            Timestamp = DateTime.UtcNow
        };

        await swipesRepository.AddSwipeAsync(swipeEntity, cancellationToken);

        await context.RespondAsync(new PostSwipeResponse(true, null));
    }
}