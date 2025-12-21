using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources; // Often needed to set service details
using OpenTelemetry.Trace;
using OrderService.Services;

// Use Host.CreateApplicationBuilder for modern .NET 8 console/worker apps
var builder = Host.CreateApplicationBuilder(args);

// Define your service name once for all signals
const string serviceName = "MyConsoleApp";

// 1. Configure the Resource Builder (Crucial for service identification)
// You can use the ConfigureResource extension method here, or do it inside each signal.
var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: "1.0.0"); // Add resource details

// 1. Configure the Resource Builder on the main AddOpenTelemetry call.
// This ensures the service name and attributes are automatically applied
// to Tracing and Metrics.
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: serviceName,
            serviceVersion: "1.0.0",
            serviceInstanceId: Environment.MachineName) // Add environment details
        .AddTelemetrySdk() // Adds OpenTelemetry SDK metadata
        .AddEnvironmentVariableDetector()) // Detects service details from ENV vars

    // Configure Tracing (The 'T' in LGTM)
    .WithTracing(tracing => tracing
        .AddHttpClientInstrumentation()
        .AddSource(serviceName)
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://otel-collector:4317");
        }))

    // Configure Metrics (The 'M' in LGTM)
    .WithMetrics(metrics => metrics
        .AddRuntimeInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri("http://otel-collector:4317");
        }));

// 3. Configure Logging (The 'L' in LGTM)
builder.Logging.AddOpenTelemetry(logging => logging
    .SetResourceBuilder(resourceBuilder) // Apply resource builder to logging
    .AddOtlpExporter(otlpOptions =>
    {
        otlpOptions.Endpoint = new Uri("http://otel-collector:4317");
    }));
builder.Services.AddHttpClient();
builder.Services.AddTransient<OrderProcessingService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// If you have a long-running app (like a Worker), use host.RunAsync();
await host.RunAsync();