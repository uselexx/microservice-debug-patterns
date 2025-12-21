using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace OrderService.Services;

// 1. Define the ActivitySource. This must match the string used in your Program.cs
// tracing configuration (e.g., tracing.AddSource("MyConsoleApp")).
public static class Telemetry
{
    // The Source is what creates new Traces/Spans.
    public static readonly ActivitySource ActivitySource = new("MyConsoleApp");
}

public class OrderProcessingService
{
    private readonly ILogger<OrderProcessingService> _logger;
    //private readonly HttpClient _httpClient; // Example dependency

    // 2. Inject ILogger<T> via the constructor (Standard Dependency Injection)
    public OrderProcessingService(ILogger<OrderProcessingService> logger)
    {
        _logger = logger;
        //_httpClient = httpClient;
    }

    public async Task ProcessOrderAsync(int orderId)
    {
        // 3. Create a new Trace/Span for this operation
        // This Activity becomes the "parent" span, and all logs/traces within it are correlated.
        using (var activity = Telemetry.ActivitySource.StartActivity("ProcessOrder"))
        {
            // Add custom attributes (key-value pairs) to the Span/Trace
            activity?.SetTag("order.id", orderId);
            _logger.LogInformation("Starting order processing for OrderId: {OrderId}", orderId);

            try
            {
                // Simulate an internal operation or service call
                await SimulateInventoryCheck(orderId);

                // 4. Use ILogger<T> normally
                _logger.LogWarning("Inventory check succeeded, but stock is low.");

                // Simulate a network call to another service
                //var response = await _httpClient.GetAsync($"http://inventoryService/api/check/{orderId}");
                var responseSuccess = true;

                if (!responseSuccess)
                {
                    // Log an Error. This log record will automatically include the current TraceId and SpanId.
                    _logger.LogError("Failed to communicate with Inventory Service. Status: {StatusCode}", responseSuccess);
                    // The error is now correlated with the "ProcessOrder" trace.
                }

                _logger.LogInformation("Order {OrderId} successfully processed.", orderId);
                activity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "CRITICAL ERROR processing order {OrderId}", orderId);
                // Mark the Trace/Span as failed
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            }
        } // End of using block disposes the Activity, ending the Trace/Span
    }

    private async Task SimulateInventoryCheck(int orderId)
    {
        // 5. You can create child Spans that automatically link to the parent "ProcessOrder" Span
        using var activity = Telemetry.ActivitySource.StartActivity("InventoryCheck");
        activity?.SetTag("inventory.checked", true);
        await Task.Delay(100); // Simulate work
    }
}
