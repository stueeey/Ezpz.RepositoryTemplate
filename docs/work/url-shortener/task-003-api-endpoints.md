# Task 003: Create REST API Endpoints

[← Back to Task Summary](./task-summary.md)

**Size**: Medium (3 files) - 🎯 Low Risk, 💪 Easy Effort  
**Dependencies**: Task 002 (Core services must be complete)

## Objective

Create REST API controllers for URL shortening operations and URL redirection.

## Implementation Steps

1. Create `Controllers/ShortenController.cs`
   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using Company.Platform.UrlShortener.Contracts;
   using Company.Platform.UrlShortener.App.Services;
   
   namespace Company.Platform.UrlShortener.App.Controllers;
   
   [ApiController]
   [Route("api/[controller]")]
   public class ShortenController : ControllerBase
   {
       private readonly IUrlShortenerService _urlShortener;
       private readonly ILogger<ShortenController> _logger;
       
       public ShortenController(IUrlShortenerService urlShortener, ILogger<ShortenController> logger)
       {
           _urlShortener = urlShortener;
           _logger = logger;
       }
       
       /// <summary>
       /// Creates a new short URL
       /// </summary>
       [HttpPost]
       [ProducesResponseType(typeof(ShortUrlResponse), StatusCodes.Status201Created)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortUrlRequest request)
       {
           if (!IsValidUrl(request.OriginalUrl))
           {
               return BadRequest(new { error = "Invalid URL format" });
           }
           
           var result = await _urlShortener.CreateShortUrlAsync(request.OriginalUrl, request.ExpiresAt);
           
           return CreatedAtAction(
               nameof(UrlsController.GetUrlDetails), 
               "Urls", 
               new { shortCode = result.ShortCode }, 
               result);
       }
       
       private bool IsValidUrl(string url)
       {
           return Uri.TryCreate(url, UriKind.Absolute, out var uri) 
               && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
       }
   }
   ```

2. Create `Controllers/RedirectController.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Controllers;
   
   [ApiController]
   public class RedirectController : ControllerBase
   {
       private readonly IUrlShortenerService _urlShortener;
       
       public RedirectController(IUrlShortenerService urlShortener)
       {
           _urlShortener = urlShortener;
       }
       
       /// <summary>
       /// Redirects to the original URL
       /// </summary>
       [HttpGet("{shortCode}")]
       [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<IActionResult> RedirectToUrl(string shortCode)
       {
           var originalUrl = await _urlShortener.GetOriginalUrlAsync(shortCode);
           
           if (originalUrl == null)
           {
               return NotFound();
           }
           
           // Increment click count asynchronously (fire-and-forget)
           _ = _urlShortener.IncrementClickCountAsync(shortCode);
           
           return RedirectPermanent(originalUrl);
       }
   }
   ```

3. Create `Controllers/UrlsController.cs`
   ```csharp
   namespace Company.Platform.UrlShortener.App.Controllers;
   
   [ApiController]
   [Route("api/[controller]")]
   public class UrlsController : ControllerBase
   {
       private readonly IUrlShortenerService _urlShortener;
       
       public UrlsController(IUrlShortenerService urlShortener)
       {
           _urlShortener = urlShortener;
       }
       
       /// <summary>
       /// Gets details about a short URL
       /// </summary>
       [HttpGet("{shortCode}")]
       [ProducesResponseType(typeof(ShortUrlResponse), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<IActionResult> GetUrlDetails(string shortCode)
       {
           var details = await _urlShortener.GetUrlDetailsAsync(shortCode);
           
           if (details == null)
           {
               return NotFound();
           }
           
           return Ok(details);
       }
       
       /// <summary>
       /// Gets statistics for a short URL
       /// </summary>
       [HttpGet("{shortCode}/stats")]
       [ProducesResponseType(typeof(UrlStatsResponse), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<IActionResult> GetUrlStats(string shortCode)
       {
           var stats = await _urlShortener.GetUrlStatsAsync(shortCode);
           
           if (stats == null)
           {
               return NotFound();
           }
           
           return Ok(stats);
       }
   }
   ```

4. Add Swagger documentation in `Program.cs`
   ```csharp
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen(c =>
   {
       c.SwaggerDoc("v1", new OpenApiInfo 
       { 
           Title = "URL Shortener API", 
           Version = "v1" 
       });
   });
   
   // In Configure section
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }
   ```

## Validation Criteria

- [ ] POST /api/shorten creates short URLs successfully
- [ ] GET /{shortCode} redirects to original URL
- [ ] GET /api/urls/{shortCode} returns URL details
- [ ] GET /api/urls/{shortCode}/stats returns click statistics
- [ ] Invalid URLs return 400 Bad Request
- [ ] Non-existent short codes return 404 Not Found
- [ ] Swagger documentation available at /swagger

## Notes

- RedirectPermanent (301) allows browser caching for performance
- Click count increment is fire-and-forget to avoid slowing redirects
- Consider adding rate limiting to POST endpoint (see Task 004)
- May want to add authorization for stats endpoint