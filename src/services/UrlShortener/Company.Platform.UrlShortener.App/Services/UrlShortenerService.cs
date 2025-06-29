using Company.Platform.UrlShortener.App.Data;
using Company.Platform.UrlShortener.App.Data.Entities;
using Company.Platform.UrlShortener.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Company.Platform.UrlShortener.App.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly UrlShortenerDbContext _context;
    private readonly IShortCodeGenerator _codeGenerator;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UrlShortenerService> _logger;
    private static readonly ActivitySource ActivitySource = new("UrlShortener");

    private const string CacheKeyPrefix = "url:";
    private static readonly MemoryCacheEntryOptions DefaultCacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(5),
        Size = 1
    };

    public UrlShortenerService(
        UrlShortenerDbContext context,
        IShortCodeGenerator codeGenerator,
        IMemoryCache cache,
        ILogger<UrlShortenerService> logger)
    {
        _context = context;
        _codeGenerator = codeGenerator;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null)
    {
        using var activity = ActivitySource.StartActivity("CreateShortUrl");
        activity?.SetTag("original_url", originalUrl);
        activity?.SetTag("has_expiration", expiresAt.HasValue);

        try
        {
            _logger.LogInformation("Creating short URL for {OriginalUrl} with expiration {ExpiresAt}", 
                originalUrl, expiresAt);

            // Create entity
            var shortUrl = new ShortUrl
            {
                ShortCode = "temp", // Will be updated after we get the ID
                OriginalUrl = originalUrl,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                ClickCount = 0
            };

            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();

            // Generate short code from ID
            shortUrl.ShortCode = _codeGenerator.GenerateShortCode(shortUrl.Id);
            await _context.SaveChangesAsync();

            // Pre-populate cache
            var cacheKey = $"{CacheKeyPrefix}{shortUrl.ShortCode}";
            _cache.Set(cacheKey, originalUrl, DefaultCacheOptions);

            _logger.LogInformation("Created short URL {ShortCode} for {OriginalUrl}", 
                shortUrl.ShortCode, originalUrl);

            // Add custom metrics
            activity?.SetTag("shortcode", shortUrl.ShortCode);
            activity?.SetTag("database_id", shortUrl.Id);

            return new ShortUrlResponse(
                shortUrl.ShortCode,
                $"/{shortUrl.ShortCode}", // Relative URL, will be made absolute by controller
                shortUrl.OriginalUrl,
                shortUrl.CreatedAt,
                shortUrl.ExpiresAt,
                shortUrl.ClickCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create short URL for {OriginalUrl}", originalUrl);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public async Task<string?> GetOriginalUrlAsync(string shortCode)
    {
        using var activity = ActivitySource.StartActivity("GetOriginalUrl");
        activity?.SetTag("shortcode", shortCode);

        var cacheKey = $"{CacheKeyPrefix}{shortCode}";

        // Try cache first
        if (_cache.TryGetValue<string>(cacheKey, out var cachedUrl))
        {
            _logger.LogDebug("Cache hit for short code: {ShortCode}", shortCode);
            activity?.SetTag("cache_hit", true);
            return cachedUrl;
        }

        activity?.SetTag("cache_hit", false);

        // Load from database
        var shortUrl = await _context.ShortUrls
            .AsNoTracking()
            .Where(u => u.ShortCode == shortCode)
            .Where(u => u.ExpiresAt == null || u.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (shortUrl != null)
        {
            // Add to cache
            _cache.Set(cacheKey, shortUrl.OriginalUrl, DefaultCacheOptions);
            _logger.LogDebug("Added to cache: {ShortCode}", shortCode);
            activity?.SetTag("found", true);
            return shortUrl.OriginalUrl;
        }

        activity?.SetTag("found", false);
        _logger.LogDebug("Short code not found or expired: {ShortCode}", shortCode);
        return null;
    }

    public async Task<ShortUrlResponse?> GetUrlDetailsAsync(string shortCode)
    {
        var shortUrl = await _context.ShortUrls
            .AsNoTracking()
            .Where(u => u.ShortCode == shortCode)
            .FirstOrDefaultAsync();

        if (shortUrl == null)
            return null;

        return new ShortUrlResponse(
            shortUrl.ShortCode,
            $"/{shortUrl.ShortCode}",
            shortUrl.OriginalUrl,
            shortUrl.CreatedAt,
            shortUrl.ExpiresAt,
            shortUrl.ClickCount);
    }

    public async Task<UrlStatsResponse?> GetUrlStatsAsync(string shortCode)
    {
        var shortUrl = await _context.ShortUrls
            .AsNoTracking()
            .Where(u => u.ShortCode == shortCode)
            .FirstOrDefaultAsync();

        if (shortUrl == null)
            return null;

        return new UrlStatsResponse(
            shortUrl.ShortCode,
            shortUrl.OriginalUrl,
            shortUrl.ClickCount,
            shortUrl.CreatedAt,
            shortUrl.LastClickedAt);
    }

    public async Task IncrementClickCountAsync(string shortCode)
    {
        using var activity = ActivitySource.StartActivity("IncrementClickCount");
        activity?.SetTag("shortcode", shortCode);

        try
        {
            var shortUrl = await _context.ShortUrls
                .Where(u => u.ShortCode == shortCode)
                .FirstOrDefaultAsync();

            if (shortUrl != null)
            {
                shortUrl.ClickCount++;
                shortUrl.LastClickedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogDebug("Incremented click count for {ShortCode} to {ClickCount}", 
                    shortCode, shortUrl.ClickCount);
                
                activity?.SetTag("click_count", shortUrl.ClickCount);
                activity?.SetTag("found", true);
            }
            else
            {
                _logger.LogWarning("Attempted to increment click count for non-existent {ShortCode}", shortCode);
                activity?.SetTag("found", false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to increment click count for {ShortCode}", shortCode);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            // Don't throw - this is fire-and-forget
        }
    }
}