using Company.Platform.UrlShortener.App.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<UrlShortenerDbContext>(options =>
            options.UseSqlite(context.Configuration.GetConnectionString("UrlShortener")
                ?? "Data Source=urlshortener.db"));
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UrlShortenerDbContext>();
    await dbContext.Database.MigrateAsync();
    Console.WriteLine("Migrations applied successfully!");
}