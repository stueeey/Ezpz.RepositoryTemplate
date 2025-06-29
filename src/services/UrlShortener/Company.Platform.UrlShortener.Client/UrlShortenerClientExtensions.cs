using Company.Platform.UrlShortener.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection;

public static class UrlShortenerClientExtensions
{
    public static IServiceCollection AddUrlShortenerClient(
        this IServiceCollection services, 
        Uri baseAddress)
    {
        services.AddHttpClient<IUrlShortenerClient, UrlShortenerClient>(client =>
        {
            client.BaseAddress = baseAddress;
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
        
        return services;
    }
    
    public static IServiceCollection AddUrlShortenerClient(
        this IServiceCollection services, 
        string baseAddress)
    {
        return services.AddUrlShortenerClient(new Uri(baseAddress));
    }
}