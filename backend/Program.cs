using backend.Hubs;
using backend.Services;
using backend.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<IDashboardService, DashboardService>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapHub<DashboardHub>("/hubs/dashboard");

// Map weather endpoints from dedicated file
app.MapWeather();

// Start broadcasting updates in the background
_ = Task.Run(async () =>
{
    var cts = new CancellationTokenSource();
    var dashboardService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IDashboardService>();
    await dashboardService.StartBroadcastingUpdates(cts.Token);
});

app.Run();
