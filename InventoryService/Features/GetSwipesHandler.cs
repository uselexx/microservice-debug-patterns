using InventoryService.Repositories;
using MassTransit;
using Shared.Contracts;

namespace InventoryService.Features;

public class GetSwipesHandler(ISwipesRepository swipesRepository) : IConsumer<GetSwipesRequest>
{
    public async Task Consume(ConsumeContext<GetSwipesRequest> context)
    {
        var request = context.Message;
        var cancellationToken = context.CancellationToken;

        var swipeEntities = await swipesRepository.GetByUserIdAsync(
            request.UserId,
            cancellationToken);

        var swipeDtos = swipeEntities
            .Select(x => new SwipeDto(
                x.Id,
                x.MovieId,
                x.IsLiked,
                x.Timestamp))
            .ToList();
            
        await context.RespondAsync(new GetSwipesResponse(swipeDtos));
    }
}