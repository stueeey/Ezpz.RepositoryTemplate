# Task 002: Implement Core Business Logic

[← Back to Task Summary](./task-summary.md)

**Size**: Medium (4 files) - 🎯 Low Risk, 💪 Easy Effort  
**Dependencies**: Task 001 (Database setup must be complete)

## Objective

Create the core services for URL shortening business logic, including short code generation and URL management.

## Implementation Steps

1. Create `Services/IUrlShortenerService.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Services;
   
   public interface IUrlShortenerService
   {
       Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null);
       Task<string?> GetOriginalUrlAsync(string shortCode);
       Task<ShortUrlResponse?> GetUrlDetailsAsync(string shortCode);
       Task<UrlStatsResponse?> GetUrlStatsAsync(string shortCode);
       Task IncrementClickCountAsync(string shortCode);
   }
   ```

2. Create `Services/IShortCodeGenerator.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Services;
   
   public interface IShortCodeGenerator
   {
       string GenerateShortCode(int id);
       int? DecodeShortCode(string shortCode);
   }
   ```

3. Create `Services/ShortCodeGenerator.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Services;
   
   public class ShortCodeGenerator : IShortCodeGenerator
   {
       private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
       private const int MinLength = 6;
       
       public string GenerateShortCode(int id)
       {
           var result = new StringBuilder();
           var value = id;
           
           while (value > 0)
           {
               result.Insert(0, Alphabet[value % Alphabet.Length]);
               value /= Alphabet.Length;
           }
           
           // Pad to minimum length
           while (result.Length < MinLength)
           {
               result.Insert(0, Alphabet[0]);
           }
           
           return result.ToString();
       }
       
       public int? DecodeShortCode(string shortCode)
       {
           // Implementation for decoding
       }
   }
   ```

4. Create `Services/UrlShortenerService.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Services;
   
   public class UrlShortenerService : IUrlShortenerService
   {
       private readonly UrlShortenerDbContext _context;
       private readonly IShortCodeGenerator _codeGenerator;
       private readonly ILogger<UrlShortenerService> _logger;
       
       public UrlShortenerService(
           UrlShortenerDbContext context,
           IShortCodeGenerator codeGenerator,
           ILogger<UrlShortenerService> logger)
       {
           _context = context;
           _codeGenerator = codeGenerator;
           _logger = logger;
       }
       
       public async Task<ShortUrlResponse> CreateShortUrlAsync(string originalUrl, DateTime? expiresAt = null)
       {
           // 1. Validate URL format
           // 2. Create entity with temporary ID
           // 3. Save to get real ID
           // 4. Generate short code from ID
           // 5. Update entity with short code
           // 6. Return response
       }
       
       // Implement other methods...
   }
   ```

5. Update `Program.cs` to register services
   ```csharp
   builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
   builder.Services.AddSingleton<IShortCodeGenerator, ShortCodeGenerator>();
   ```

## Validation Criteria

- [ ] Short codes are unique and at least 6 characters
- [ ] URL validation rejects invalid formats
- [ ] Service handles concurrent requests safely
- [ ] Expired URLs return null when retrieved
- [ ] Click count increments are atomic
- [ ] All methods have proper error handling and logging

## Notes

- Base62 encoding provides good density while remaining URL-safe
- Minimum length of 6 provides ~56 billion combinations
- Consider adding URL normalization (trailing slashes, etc.)
- May want to check for existing URLs to avoid duplicates