using Microsoft.AspNetCore.Mvc;
using Company.Platform.UrlShortener.App.Services;

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