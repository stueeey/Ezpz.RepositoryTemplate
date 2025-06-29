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