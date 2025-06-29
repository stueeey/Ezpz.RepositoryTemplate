namespace Company.Platform.UrlShortener.Contracts;

public record ShortUrlResponse(
    string ShortCode,
    string ShortUrl,
    string OriginalUrl,
    DateTime CreatedAt,
    DateTime? ExpiresAt,
    int ClickCount);