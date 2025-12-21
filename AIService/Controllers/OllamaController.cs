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

    [HttpGet]
    public async Task<IActionResult> Get(string prompt)
    {
        var stopwatch = Stopwatch.StartNew(); // Starts and creates the instance in one line

        try
        {
            var response = await _service.AskMistralAsync(prompt);

            stopwatch.Stop();

            // Use structured logging with a template for better performance and searchability
            _logger.LogInformation("Mistral responded in {ElapsedMilliseconds}ms for prompt: {Prompt}",
                stopwatch.ElapsedMilliseconds, prompt);

            return Ok(new
            {
                Result = response,
                ElapsedMs = stopwatch.ElapsedMilliseconds
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Mistral call failed after {ElapsedMilliseconds}ms", stopwatch.ElapsedMilliseconds);
            return StatusCode(500, "An error occurred processing your request.");
        }
    }
}
