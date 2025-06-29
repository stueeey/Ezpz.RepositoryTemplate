# Task 004: Add Caching Layer

[← Back to Task Summary](./task-summary.md)

**Size**: Small (1 file modification) - 🎯 Very Low Risk, 💪 Easy Effort  
**Dependencies**: Task 002 (Core services must be complete)

## Objective

Add memory caching to improve performance for frequently accessed URLs.

## Implementation Steps

1. Update `Services/UrlShortenerService.cs` to add caching
   ```csharp
   using Microsoft.Extensions.Caching.Memory;
   
   public class UrlShortenerService : IUrlShortenerService
   {
       private readonly UrlShortenerDbContext _context;
       private readonly IShortCodeGenerator _codeGenerator;
       private readonly IMemoryCache _cache;
       private readonly ILogger<UrlShortenerService> _logger;
       
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
       
       public async Task<string?> GetOriginalUrlAsync(string shortCode)
       {
           var cacheKey = $"{CacheKeyPrefix}{shortCode}";
           
           // Try cache first
           if (_cache.TryGetValue<string>(cacheKey, out var cachedUrl))
           {
               _logger.LogDebug("Cache hit for short code: {ShortCode}", shortCode);
               return cachedUrl;
           }
           
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
               return shortUrl.OriginalUrl;
           }
           
           return null;
       }
       
       public async Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null)
       {
           // ... existing implementation ...
           
           // Pre-populate cache for immediate use
           var cacheKey = $"{CacheKeyPrefix}{shortUrl.ShortCode}";
           _cache.Set(cacheKey, originalUrl, DefaultCacheOptions);
           
           return response;
       }
   }
   ```

2. Update `Program.cs` to configure memory cache with size limit
   ```csharp
   builder.Services.AddMemoryCache(options =>
   {
       options.SizeLimit = 1000; // Maximum 1000 entries
   });
   ```

3. Add cache invalidation for expired URLs (optional enhancement)
   ```csharp
   public async Task InvalidateExpiredUrlsAsync()
   {
       var expiredCodes = await _context.ShortUrls
           .Where(u => u.ExpiresAt != null && u.ExpiresAt <= DateTime.UtcNow)
           .Select(u => u.ShortCode)
           .ToListAsync();
       
       foreach (var code in expiredCodes)
       {
           _cache.Remove($"{CacheKeyPrefix}{code}");
       }
   }
   ```

## Validation Criteria

- [ ] Cache hit rate visible in logs
- [ ] Cached URLs return without database query
- [ ] Cache respects expiration times
- [ ] Memory usage stays within limits
- [ ] Cache entries expire after 5 minutes of inactivity
- [ ] New URLs are pre-cached on creation

## Notes

- In-memory cache is suitable for single-instance deployments
- For multi-instance, consider Redis (would be a separate task)
- 5-minute sliding expiration balances performance and memory
- Size limit prevents unbounded memory growth
- Consider adding performance counters for cache hit/miss ratio