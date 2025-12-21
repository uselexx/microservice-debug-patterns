using InventoryService.Features;
using InventoryService.Jobs;
using InventoryService.Repositories;
using InventoryService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;

Console.WriteLine("Inventory Service starting up ..");
Console.WriteLine("Configuring Service");
Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQLConnection");

// register database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PostgreSQLConnection"),
        npgsql =>
        {
            npgsql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            npgsql.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        });

    // Optional: helpful during development
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<MovieImportService>();

// register Quartz job

// Deactivated because the job runs 22 Minutes at the moment
// Will be added later when improved
//builder.Services.AddQuartz(q =>
//{
//    var jobKey = new JobKey(nameof(MovieImportJob));

//    q.AddJob<MovieImportJob>(opts => opts.WithIdentity(jobKey));

//    // Trigger 1: run immediately on startup
//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("StartupTrigger")
//        .StartNow()
//    );

//    // Trigger 2: Run every hour
//    q.AddTrigger(opts => opts
//        .ForJob(jobKey)
//        .WithIdentity("HourlyTrigger")
//        .WithSimpleSchedule(x => x
//            .WithIntervalInHours(1)
//            .RepeatForever())
//    );
//});

//builder.Services.AddQuartzHostedService(options =>
//{
//    options.WaitForJobsToComplete = true;
//});

// Add mass transit

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GetMovieHandler>();
    x.AddConsumer<GetMoviesHandler>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

Console.WriteLine("Inventory Service finished configuring and started!");
await host.RunAsync();
