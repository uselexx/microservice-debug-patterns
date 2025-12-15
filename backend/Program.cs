using backend.Hubs;
using backend.Services;
using backend.Endpoints;
using backend.Repositories;
using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Register repositories
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IWidgetRepository, WidgetRepository>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost", "http://frontend")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddDbContext<PGContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

var app = builder.Build();

// Apply migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PGContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapHub<DashboardHub>("/hubs/dashboard");

// Map endpoints
app.MapWeather();
app.MapDashboard();

// Start broadcasting updates in the background
_ = Task.Run(async () =>
{
    var cts = new CancellationTokenSource();
    var dashboardService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IDashboardService>();
    await dashboardService.StartBroadcastingUpdates(cts.Token);
});

app.Run();
