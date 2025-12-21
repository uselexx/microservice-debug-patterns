using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AIService.Services;

public class OllamaService
{

    private readonly HttpClient _httpClient;

    public OllamaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:11434");
    }

    public async Task<string> AskMistralAsync(string prompt)
    {
        var requestBody = new
        {
            model = "mistral",
            prompt,
            stream = false // Set to false for a single string response
        };

        var response = await _httpClient.PostAsJsonAsync("/api/generate", requestBody);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
        return result?.Response ?? string.Empty;
    }
}
public record OllamaResponse(string Response);