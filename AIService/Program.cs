using AIService.Services;

var builder = WebApplication.CreateBuilder(args);

const string myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Add your exact frontend URLs
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Required for SignalR or Cookies
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddScoped<OllamaService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// FIX: Correct Middleware Order
app.UseRouting(); // 1. Must come before CORS

app.UseCors(myAllowSpecificOrigins); // 2. Must come after Routing, before Auth/Endpoints

app.UseHttpsRedirection(); // 3. Redirect to HTTPS

app.UseAuthorization(); // 4. Authorization comes last

app.MapControllers();

app.Run();