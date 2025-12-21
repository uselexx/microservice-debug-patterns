using Microsoft.Extensions.Hosting;

namespace OrderService.Services;

// Worker service to execute the logic
public class Worker : BackgroundService
{
    private readonly OrderProcessingService _processor;
    public Worker(OrderProcessingService processor) => _processor = processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken); // Wait for stack to start
        await _processor.ProcessOrderAsync(1001);
    }
}