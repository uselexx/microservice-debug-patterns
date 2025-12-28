using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace backend.Endpoints;
public static class SwipeEndpoint
{
    public static void MapSwipeEndpoints(this WebApplication app)
    {
        app.MapGetSwipesForuser();
        app.MapPostSwipe();
    }

    private static void MapGetSwipesForuser(this WebApplication app)
    {
        app.MapGet("/swipes/{userId}", async (string userId, IRequestClient<GetSwipesRequest> requestClient) =>
        {
            var request = new GetSwipesRequest(userId);

            var response = await requestClient.GetResponse<GetSwipesResponse>(request);
            
            return Results.Ok(response.Message);
        }).WithTags("Swipe");
    }

    private static void MapPostSwipe(this WebApplication app)
    {
        app.MapPost("/swipes", async ([FromBody] PostSwipeRequest request, IRequestClient<PostSwipeRequest> requestClient) =>
        {
            var response = await requestClient.GetResponse<PostSwipeResponse>(request);
            
            return Results.Ok(response.Message);
        }).WithTags("Swipe");
    }
}