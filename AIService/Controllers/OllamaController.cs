using AIService.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AIService.Controllers;

[ApiController]
[Route("[controller]")]
public class OllamaController : ControllerBase
{
    private readonly ILogger<OllamaController> _logger;
    private readonly OllamaService _service;

    public OllamaController(ILogger<OllamaController> logger, OllamaService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Get([FromBody] MistralRequest request)
    {
        var stopwatch = Stopwatch.StartNew(); // Starts and creates the instance in one line

        try
        {
            var response = await _service.AskMistralAsync(request.Prompt);

            stopwatch.Stop();

            // Use structured logging with a template for better performance and searchability
            _logger.LogInformation("Mistral responded in {ElapsedMilliseconds}ms for prompt: {Prompt}",
                stopwatch.ElapsedMilliseconds, request.Prompt);

            return Ok(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Mistral call failed after {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            return StatusCode(500, "An error occurred processing your request.");
        }
    }
}

public record MistralRequest(string Prompt);
