using Microsoft.AspNetCore.SignalR;
using backend.Hubs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Services
{
    public interface IDashboardService
    {
        Task StartBroadcastingUpdates(CancellationToken cancellationToken);
    }

    public class DashboardService : IDashboardService
    {
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly Random _random = new Random();

        public DashboardService(IHubContext<DashboardHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task StartBroadcastingUpdates(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var update = new
                    {
                        timestamp = DateTime.UtcNow,
                        cpuUsage = _random.Next(10, 95),
                        memoryUsage = _random.Next(20, 80),
                        requestCount = _random.Next(100, 5000),
                        status = new[] { "Healthy", "Warning", "Critical" }[_random.Next(3)]
                    };

                    await _hubContext.Clients.All.SendAsync("ReceiveUpdate", update, cancellationToken);
                    await Task.Delay(2000, cancellationToken); // Send update every 2 seconds
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error broadcasting update: {ex.Message}");
                }
            }
        }
    }
}
