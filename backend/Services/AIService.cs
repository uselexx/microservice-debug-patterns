using Shared.Contracts;
using System.Net.Http;

namespace backend.Services;

public class AIService(HttpClient httpClient, ILogger<AIService> logger)
{
    public async Task<string> CallLLM(string prompt)
    {
        var requestBody = new MistralRequest(prompt);

        // Using a Scope allows you to track this specific request across multiple log lines
        using var scope = logger.BeginScope(new Dictionary<string, object> { ["Prompt"] = prompt });

        logger.LogInformation("AI Service Request: Initiating POST to /Ollama");

        try
        {
            var response = await httpClient.PostAsJsonAsync("Ollama", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                // IMPORTANT: Read the error body from the AIService to see why it failed
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("AI Service Failed. Status: {StatusCode}, Reason: {Reason}, Body: {Body}",
                    (int)response.StatusCode, response.ReasonPhrase, errorContent);

                response.EnsureSuccessStatusCode(); // This will still throw, but we logged the details first
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            logger.LogInformation("AI Service Request: Success. Received {Length} characters.", stringResponse?.Length ?? 0);

            return stringResponse ?? string.Empty;
        }
        catch (HttpRequestException ex)
        {
            logger.LogCritical(ex, "Network or Protocol error while calling AI Service at {BaseAddress}", httpClient.BaseAddress);
            throw;
        }
    }
}
