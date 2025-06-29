# Task 006: Implement Comprehensive Tests

[← Back to Task Summary](./task-summary.md)

**Size**: Large (6 files) - 🎯 Low Risk, 💪 Medium Effort  
**Dependencies**: Tasks 001-003 must be complete (Database, Services, API)

## Objective

Create unit, integration, and E2E tests to ensure the URL shortener works correctly.

## Implementation Steps

### 1. Unit Tests - `ShortCodeGeneratorTests.cs` in App.Tests
```csharp
using Company.Platform.UrlShortener.App.Services;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;

namespace Company.Platform.UrlShortener.App.Tests.Services;

[TestClass]
public class ShortCodeGeneratorTests
{
    private readonly ShortCodeGenerator _generator = new();

    [Test]
    [Category("Unit")]
    [DisplayName("Given_Id_When_GenerateShortCode_Then_Creates_MinimumLength_Code")]
    public void Given_Id_When_GenerateShortCode_Then_Creates_MinimumLength_Code()
    {
        // Arrange
        var id = 1;
        
        // Act
        var result = _generator.GenerateShortCode(id);
        
        // Assert
        result.Should().HaveLength(6);
        result.Should().MatchRegex("^[a-zA-Z0-9]+$");
    }
    
    [Test]
    [TestCase(1, "aaaaab")]
    [TestCase(62, "aaaaba")]
    [TestCase(1000000, "aaemjc")]
    [Category("Unit")]
    [DisplayName("Given_KnownId_When_GenerateShortCode_Then_Returns_DeterministicResult")]
    public void Given_KnownId_When_GenerateShortCode_Then_Returns_DeterministicResult(int id, string expected)
    {
        // Act
        var result = _generator.GenerateShortCode(id);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Test]
    [Category("Unit")]
    [DisplayName("Given_ShortCode_When_DecodeShortCode_Then_Reverses_Encoding")]
    public void Given_ShortCode_When_DecodeShortCode_Then_Reverses_Encoding()
    {
        // Arrange
        var id = 123456;
        var shortCode = _generator.GenerateShortCode(id);
        
        // Act
        var decodedId = _generator.DecodeShortCode(shortCode);
        
        // Assert
        decodedId.Should().Be(id);
    }
}
```

### 2. Unit Tests - `UrlShortenerServiceTests.cs` in App.Tests
```csharp
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

[TestClass]
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
    [Category("Unit")]
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
    [Category("Unit")]
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
    
    public void Dispose()
    {
        _context.Dispose();
        _cache.Dispose();
    }
}
```

### 3. API Tests - `ShortenControllerTests.cs` in ApiTests
```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;
using Company.Platform.UrlShortener.Contracts;
using Company.Platform.UrlShortener.App;

namespace Company.Platform.UrlShortener.ApiTests;

[TestClass]
public class ShortenControllerTests : IDisposable
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    
    [SetUp]
    public async Task Setup()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    [Test]
    [Category("Integration")]
    [DisplayName("Given_ValidUrl_When_CreateShortUrl_Then_Returns_Created")]
    public async Task Given_ValidUrl_When_CreateShortUrl_Then_Returns_Created()
    {
        // Arrange
        var request = new CreateShortUrlRequest("https://example.com", null);
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/shorten", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadFromJsonAsync<ShortUrlResponse>();
        content.Should().NotBeNull();
        content!.ShortCode.Should().NotBeNullOrEmpty();
        content.ShortUrl.Should().Contain(content.ShortCode);
    }
    
    [Test]
    [Category("Integration")]
    [DisplayName("Given_InvalidUrl_When_CreateShortUrl_Then_Returns_BadRequest")]
    public async Task Given_InvalidUrl_When_CreateShortUrl_Then_Returns_BadRequest()
    {
        // Arrange
        var request = new CreateShortUrlRequest("not-a-url", null);
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/shorten", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
    
    public void Dispose()
    {
        TearDown();
    }
}
```

### 4. E2E Tests - `UrlShortenerE2ETests.cs` in E2ETests
```csharp
using Microsoft.Playwright;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;
using Company.Platform.UrlShortener.Client;
using Microsoft.Extensions.Logging;

namespace Company.Platform.UrlShortener.E2ETests;

[TestClass]
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
    [Category("E2E")]
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
```

## Validation Criteria

- [ ] Unit tests cover all service methods
- [ ] Unit tests use in-memory database
- [ ] API tests verify all endpoints
- [ ] API tests check error cases
- [ ] E2E test verifies full user flow
- [ ] All tests are independent and repeatable
- [ ] Test naming follows Given_When_Then pattern

## Notes

- Use in-memory database for unit tests to avoid I/O
- WebApplicationFactory provides real HTTP testing
- E2E tests may need Aspire running locally
- Consider adding performance tests for high load
- Mock external dependencies (if any) in unit tests