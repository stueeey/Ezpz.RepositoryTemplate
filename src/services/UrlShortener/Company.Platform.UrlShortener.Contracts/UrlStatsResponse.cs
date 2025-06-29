namespace Company.Platform.UrlShortener.Contracts;

public record UrlStatsResponse(
    string ShortCode,
    string OriginalUrl,
    int TotalClicks,
    DateTime CreatedAt,
    DateTime? LastClickedAt);