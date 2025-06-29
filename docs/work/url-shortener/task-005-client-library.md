# Task 005: Create HTTP Client Library

[← Back to Task Summary](./task-summary.md)

**Size**: Medium (2 files) - 🎯 Low Risk, 💪 Easy Effort  
**Dependencies**: Task 003 (API endpoints must be complete)

## Objective

Create a reusable HTTP client library that other services can use to interact with the URL shortener.

## Implementation Steps

1. Create `IUrlShortenerClient.cs` in the Client project
   ```csharp
   using Company.Platform.UrlShortener.Contracts;
   
   namespace Company.Platform.UrlShortener.Client;
   
   public interface IUrlShortenerClient
   {
       /// <summary>
       /// Creates a new short URL
       /// </summary>
       Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null, CancellationToken cancellationToken = default);
       
       /// <summary>
       /// Gets details about a short URL
       /// </summary>
       Task<ShortUrlResponse?> GetUrlDetailsAsync(string shortCode, CancellationToken cancellationToken = default);
       
       /// <summary>
       /// Gets statistics for a short URL
       /// </summary>
       Task<UrlStatsResponse?> GetUrlStatsAsync(string shortCode, CancellationToken cancellationToken = default);
       
       /// <summary>
       /// Gets the full URL for a short code (without redirecting)
       /// </summary>
       Task<string?> GetOriginalUrlAsync(string shortCode, CancellationToken cancellationToken = default);
   }
   ```

2. Create `UrlShortenerClient.cs`
   ```csharp
   using System.Net.Http.Json;
   using Microsoft.Extensions.Logging;
   using Company.Platform.UrlShortener.Contracts;
   
   namespace Company.Platform.UrlShortener.Client;
   
   public class UrlShortenerClient : IUrlShortenerClient
   {
       private readonly HttpClient _httpClient;
       private readonly ILogger<UrlShortenerClient> _logger;
       
       public UrlShortenerClient(HttpClient httpClient, ILogger<UrlShortenerClient> logger)
       {
           _httpClient = httpClient;
           _logger = logger;
       }
       
       public async Task<ShortUrlResponse> CreateShortUrlAsync(
           string originalUrl, 
           DateTime? expiresAt = null, 
           CancellationToken cancellationToken = default)
       {
           var request = new CreateShortUrlRequest(originalUrl, expiresAt);
           
           var response = await _httpClient.PostAsJsonAsync(
               "api/shorten", 
               request, 
               cancellationToken);
           
           response.EnsureSuccessStatusCode();
           
           var result = await response.Content.ReadFromJsonAsync<ShortUrlResponse>(cancellationToken);
           return result ?? throw new InvalidOperationException("Received null response");
       }
       
       public async Task<ShortUrlResponse?> GetUrlDetailsAsync(
           string shortCode, 
           CancellationToken cancellationToken = default)
       {
           var response = await _httpClient.GetAsync(
               $"api/urls/{shortCode}", 
               cancellationToken);
           
           if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
           {
               return null;
           }
           
           response.EnsureSuccessStatusCode();
           return await response.Content.ReadFromJsonAsync<ShortUrlResponse>(cancellationToken);
       }
       
       public async Task<UrlStatsResponse?> GetUrlStatsAsync(
           string shortCode, 
           CancellationToken cancellationToken = default)
       {
           var response = await _httpClient.GetAsync(
               $"api/urls/{shortCode}/stats", 
               cancellationToken);
           
           if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
           {
               return null;
           }
           
           response.EnsureSuccessStatusCode();
           return await response.Content.ReadFromJsonAsync<UrlStatsResponse>(cancellationToken);
       }
       
       public async Task<string?> GetOriginalUrlAsync(
           string shortCode, 
           CancellationToken cancellationToken = default)
       {
           var details = await GetUrlDetailsAsync(shortCode, cancellationToken);
           return details?.OriginalUrl;
       }
   }
   ```

3. Create extension method for DI registration
   ```csharp
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
   ```

## Validation Criteria

- [ ] Client can create short URLs
- [ ] Client handles 404 responses gracefully (returns null)
- [ ] Client includes proper error handling and logging
- [ ] HttpClient is configured with base address
- [ ] Cancellation tokens are properly propagated
- [ ] Extension method simplifies DI registration

## Notes

- Uses typed HttpClient for proper lifecycle management
- Returns null for 404s to match service behavior
- Includes cancellation token support for all operations
- Consider adding Polly for retry policies in production
- Base address should include trailing slash