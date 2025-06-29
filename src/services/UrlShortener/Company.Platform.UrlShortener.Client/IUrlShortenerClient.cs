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