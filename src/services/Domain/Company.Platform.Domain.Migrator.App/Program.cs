using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add configuration
builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

// Add services
builder.Services.AddHostedService<MigrationService>();

var host = builder.Build();

await host.RunAsync();

public class MigrationService : IHostedService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<MigrationService> _logger;

    public MigrationService(ILogger<MigrationService> logger, IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _lifetime = lifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database migration...");

        try
        {
            // TODO: Add actual migration logic here
            // For now, this is just a placeholder

            _logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database migration failed");
            throw;
        }
        finally
        {
            // Stop the application after migration completes
            _lifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}