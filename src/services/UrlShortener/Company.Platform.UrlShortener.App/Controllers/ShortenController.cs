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

        // Build absolute URL
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var absoluteResult = result with { ShortUrl = $"{baseUrl}{result.ShortUrl}" };

        return CreatedAtAction(
            nameof(UrlsController.GetUrlDetails),
            "Urls",
            new { shortCode = result.ShortCode },
            absoluteResult);
    }

    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}