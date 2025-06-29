using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using FakeItEasy;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;
using Company.Platform.UrlShortener.App.Data;
using Company.Platform.UrlShortener.App.Data.Entities;
using Company.Platform.UrlShortener.App.Services;

namespace Company.Platform.UrlShortener.App.Tests.Services;

public class UrlShortenerServiceTests : IDisposable
{
    private readonly UrlShortenerDbContext _context;
    private readonly UrlShortenerService _service;
    private readonly IMemoryCache _cache;
    
    public UrlShortenerServiceTests()
    {
        var options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _context = new UrlShortenerDbContext(options);
        _cache = new MemoryCache(new MemoryCacheOptions());
        
        _service = new UrlShortenerService(
            _context,
            new ShortCodeGenerator(),
            _cache,
            A.Fake<ILogger<UrlShortenerService>>());
    }
    
    [Test]
    [DisplayName("Given_ValidUrl_When_CreateShortUrlAsync_Then_Generates_Unique_Code")]
    public async Task Given_ValidUrl_When_CreateShortUrlAsync_Then_Generates_Unique_Code()
    {
        // Arrange
        var url = "https://example.com";
        
        // Act
        var result = await _service.CreateShortUrlAsync(url);
        
        // Assert
        result.ShortCode.Should().NotBeNullOrEmpty();
        result.OriginalUrl.Should().Be(url);
        result.ClickCount.Should().Be(0);
        
        var dbEntry = await _context.ShortUrls.FirstAsync();
        dbEntry.ShortCode.Should().Be(result.ShortCode);
    }
    
    [Test]
    [DisplayName("Given_ExpiredUrl_When_GetOriginalUrlAsync_Then_Returns_Null")]
    public async Task Given_ExpiredUrl_When_GetOriginalUrlAsync_Then_Returns_Null()
    {
        // Arrange
        var shortUrl = new ShortUrl
        {
            ShortCode = "abc123",
            OriginalUrl = "https://example.com",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            ClickCount = 0
        };
        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync();
        
        // Act
        var result = await _service.GetOriginalUrlAsync("abc123");
        
        // Assert
        result.Should().BeNull();
    }
    
    [Test]
    [DisplayName("Given_ValidShortCode_When_GetOriginalUrlAsync_Then_Returns_CachedUrl")]
    public async Task Given_ValidShortCode_When_GetOriginalUrlAsync_Then_Returns_CachedUrl()
    {
        // Arrange
        var originalUrl = "https://example.com";
        var shortCode = "test123";
        
        // Pre-populate cache
        _cache.Set($"url:{shortCode}", originalUrl);
        
        // Act
        var result = await _service.GetOriginalUrlAsync(shortCode);
        
        // Assert
        result.Should().Be(originalUrl);
    }
    
    [Test]
    [DisplayName("Given_ShortCode_When_IncrementClickCountAsync_Then_Updates_Statistics")]
    public async Task Given_ShortCode_When_IncrementClickCountAsync_Then_Updates_Statistics()
    {
        // Arrange
        var shortUrl = new ShortUrl
        {
            ShortCode = "test123",
            OriginalUrl = "https://example.com",
            CreatedAt = DateTime.UtcNow,
            ClickCount = 0
        };
        _context.ShortUrls.Add(shortUrl);
        await _context.SaveChangesAsync();
        
        // Act
        await _service.IncrementClickCountAsync("test123");
        
        // Assert
        var updated = await _context.ShortUrls.FirstAsync(u => u.ShortCode == "test123");
        updated.ClickCount.Should().Be(1);
        updated.LastClickedAt.Should().NotBeNull();
    }
    
    public void Dispose()
    {
        _context.Dispose();
        _cache.Dispose();
    }
}