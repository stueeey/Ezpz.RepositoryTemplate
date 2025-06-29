using Company.Platform.UrlShortener.App.Data;
using Company.Platform.UrlShortener.App.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Entity Framework with SQLite
builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("UrlShortener")
        ?? "Data Source=urlshortener.db"));

// Add memory cache
builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1000; // Maximum 1000 entries
});

// Add application services
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddSingleton<IShortCodeGenerator, ShortCodeGenerator>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "URL Shortener API",
        Version = "v1"
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UrlShortenerDbContext>("database")
    .AddCheck("ready", () => HealthCheckResult.Healthy("Service is ready"));

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add request logging middleware
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    
    try
    {
        await next();
    }
    finally
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation(
            "Request {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            sw.ElapsedMilliseconds);
    }
});

app.UseHttpsRedirection();

app.UseAuthorization();

// Map health check endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Run no checks, just return 200
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Name == "ready"
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();
