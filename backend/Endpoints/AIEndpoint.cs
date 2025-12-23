using backend.Services;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace backend.Endpoints;

public static class AIEndpoint
{
    public static void MapAI(this WebApplication app)
    {
        app.MapPost("/Ollama", async ([FromBody] GetResponseRequest request, AIService service) =>
        {
            // MassTransit Request-Response pattern
            var response = await service.CallLLM(request.prompt);

            // response.Message contains the actual GetMovieResponse object
            return Results.Text(response, contentType: "text/plain");
        }).WithTags("AI Models");
    }
}

public record GetResponseRequest(string prompt);