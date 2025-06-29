using Company.Platform.UrlShortener.Contracts;

namespace Company.Platform.UrlShortener.App.Services;

public interface IUrlShortenerService
{
    Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null);
    Task<string?> GetOriginalUrlAsync(string shortCode);
    Task<ShortUrlResponse?> GetUrlDetailsAsync(string shortCode);
    Task<UrlStatsResponse?> GetUrlStatsAsync(string shortCode);
    Task IncrementClickCountAsync(string shortCode);
}