using Microsoft.AspNetCore.Mvc;
using Company.Platform.UrlShortener.Contracts;
using Company.Platform.UrlShortener.App.Services;

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

        // Build absolute URL
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var absoluteDetails = details with { ShortUrl = $"{baseUrl}{details.ShortUrl}" };

        return Ok(absoluteDetails);
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