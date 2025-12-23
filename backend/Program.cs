using backend.Data;
using backend.Endpoints;
using backend.Hubs;
using backend.Repositories;
using backend.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Unified CORS Configuration
const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost", "http://frontend")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Essential for SignalR
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var aiBaseUrl = builder.Configuration.GetValue<string>("AISettings:BaseUrl");
// Register Typed Client
builder.Services.AddHttpClient<AIService>(client =>
{
    client.BaseAddress = new Uri(aiBaseUrl ?? throw new InvalidOperationException("AI Base URL is missing"));
});
// Database & Repositories
builder.Services.AddDbContext<PGContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IWidgetRepository, WidgetRepository>();

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2. Middleware Order: Routing -> CORS -> Auth -> Endpoints
app.UseHttpsRedirection();
app.UseCors(myAllowSpecificOrigins);

// 3. Map Endpoints/Hubs
app.MapHub<DashboardHub>("/hubs/dashboard");
app.MapWeather();
app.MapDashboard();
app.MapMovies();
app.MapAI();

// 4. Fixed Background Task (Safe Scoping)
_ = Task.Run(async () =>
{
    using var scope = app.Services.CreateScope();
    var dashboardService = scope.ServiceProvider.GetRequiredService<IDashboardService>();
    // Note: If this is a long-running loop, consider making it an IHostedService instead
    await dashboardService.StartBroadcastingUpdates(CancellationToken.None);
});

app.Run();