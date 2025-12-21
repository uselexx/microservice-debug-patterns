using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace backend.Endpoints;

public static class MoviesEndpoint
{
    public static void MapMovies(this WebApplication app)
    {
        app.MapGet("/movies/{cursor:int}/{pageSize:int}", async (int? cursor, int pageSize, IRequestClient<GetMoviesRequest> requestClient) =>
        {
            var request = new GetMoviesRequest(cursor, pageSize);

            var response = await requestClient.GetResponse<GetMoviesResponse>(request);
            return Results.Ok(response.Message);
        }).WithTags("Movies");

        app.MapPost("/movies", async ([FromBody] GetMovieRequest request, IRequestClient<GetMovieRequest> requestClient) =>
        {
            // MassTransit Request-Response pattern
            var response = await requestClient.GetResponse<GetMovieResponse>(request);

            // response.Message contains the actual GetMovieResponse object
            return Results.Ok(response.Message);
        }).WithTags("Movies");

        // Get the filter options
        app.MapGet("/movies/filters", async () =>
        {
            var metadata = new
            {
                Categories = new[]
                {
                    new { Value = "weather_storm", Label = "Stormy" },
                    new { Value = "weather_sunny", Label = "Sunny" },
                    new { Value = "weather_cloudy", Label = "Cloudy" }
                },
                        Tags = new[]
                {
                    new { Value = "temp-high", Label = "High Temperature" },
                    new { Value = "precip-low", Label = "Low Precipitation" },
                    new { Value = "wind-warning", Label = "Wind Warning" }
                }
            };
            return Results.Ok(metadata);
        }).WithTags("Movies");
    }
}
