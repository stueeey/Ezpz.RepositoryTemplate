namespace Company.Platform.UrlShortener.Contracts;

public record CreateShortUrlRequest(string OriginalUrl, DateTime? ExpiresAt = null);