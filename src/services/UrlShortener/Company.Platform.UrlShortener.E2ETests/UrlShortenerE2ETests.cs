using Microsoft.Playwright;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;
using Company.Platform.UrlShortener.Client;
using Microsoft.Extensions.Logging;

namespace Company.Platform.UrlShortener.E2ETests;

public class UrlShortenerE2ETests : IAsyncDisposable
{
    private IUrlShortenerClient _client;
    private IBrowser _browser;
    private IPage _page;
    private string _serviceUrl = "http://localhost:5100"; // From Aspire
    
    [SetUp]
    public async Task Setup()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri(_serviceUrl) };
        var logger = new TestLogger<UrlShortenerClient>();
        _client = new UrlShortenerClient(httpClient, logger);
        
        // Set up Playwright
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _page = await _browser.NewPageAsync();
    }
    
    [Test]
    [DisplayName("Given_ShortUrl_When_FullFlow_CreateAndRedirect_Then_RedirectsCorrectly")]
    public async Task Given_ShortUrl_When_FullFlow_CreateAndRedirect_Then_RedirectsCorrectly()
    {
        // Create short URL using client
        var originalUrl = "https://www.example.com";
        var shortUrl = await _client.CreateShortUrlAsync(originalUrl);
        
        shortUrl.Should().NotBeNull();
        shortUrl.OriginalUrl.Should().Be(originalUrl);
        
        // Navigate to short URL using Playwright
        var response = await _page.GotoAsync($"{_serviceUrl}/{shortUrl.ShortCode}");
        
        // Should redirect to original URL
        response!.Url.Should().Be(originalUrl);
        
        // Wait a moment for the async click count increment
        await Task.Delay(100);
        
        // Check stats show click
        var stats = await _client.GetUrlStatsAsync(shortUrl.ShortCode);
        stats!.TotalClicks.Should().BeGreaterThan(0);
    }
    
    [Test]
    [DisplayName("Given_ClientLibrary_When_CreateAndRetrieve_Then_WorksCorrectly")]
    public async Task Given_ClientLibrary_When_CreateAndRetrieve_Then_WorksCorrectly()
    {
        // Create URL
        var originalUrl = "https://github.com/example/repo";
        var shortUrl = await _client.CreateShortUrlAsync(originalUrl);
        
        // Verify creation
        shortUrl.Should().NotBeNull();
        shortUrl.OriginalUrl.Should().Be(originalUrl);
        shortUrl.ShortCode.Should().NotBeNullOrEmpty();
        
        // Retrieve details
        var details = await _client.GetUrlDetailsAsync(shortUrl.ShortCode);
        details.Should().NotBeNull();
        details!.OriginalUrl.Should().Be(originalUrl);
        details.ShortCode.Should().Be(shortUrl.ShortCode);
        
        // Get original URL directly
        var retrievedUrl = await _client.GetOriginalUrlAsync(shortUrl.ShortCode);
        retrievedUrl.Should().Be(originalUrl);
        
        // Get stats
        var stats = await _client.GetUrlStatsAsync(shortUrl.ShortCode);
        stats.Should().NotBeNull();
        stats!.OriginalUrl.Should().Be(originalUrl);
        stats.TotalClicks.Should().Be(0); // No redirect yet
    }
    
    [Test]
    [DisplayName("Given_NonExistentCode_When_GetDetails_Then_Returns_Null")]
    public async Task Given_NonExistentCode_When_GetDetails_Then_Returns_Null()
    {
        // Act
        var details = await _client.GetUrlDetailsAsync("nonexistent");
        var stats = await _client.GetUrlStatsAsync("nonexistent");
        var url = await _client.GetOriginalUrlAsync("nonexistent");
        
        // Assert
        details.Should().BeNull();
        stats.Should().BeNull();
        url.Should().BeNull();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        if (_page != null) await _page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        await TearDown();
    }
}

// Simple test logger implementation
public class TestLogger<T> : ILogger<T>
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => false;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
}