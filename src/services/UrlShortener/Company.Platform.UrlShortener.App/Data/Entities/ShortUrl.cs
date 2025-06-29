namespace Company.Platform.UrlShortener.App.Data.Entities;

public class ShortUrl
{
    public int Id { get; set; }
    public required string ShortCode { get; set; }
    public required string OriginalUrl { get; set; }
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? LastClickedAt { get; set; }
}