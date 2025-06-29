using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using Company.Platform.UrlShortener.Contracts;

namespace Company.Platform.UrlShortener.ApiTests;

public class ShortenControllerTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private HttpClient _client;

    public ShortenControllerTests()
    {
        _client = _factory.CreateClient();
    }

    [Test]
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

    [Test]
    [DisplayName("Given_ShortCode_When_GetUrlDetails_Then_Returns_Details")]
    public async Task Given_ShortCode_When_GetUrlDetails_Then_Returns_Details()
    {
        // Arrange - Create a URL first
        var createRequest = new CreateShortUrlRequest("https://example.com", null);
        var createResponse = await _client.PostAsJsonAsync("/api/shorten", createRequest);
        var createdUrl = await createResponse.Content.ReadFromJsonAsync<ShortUrlResponse>();

        // Act
        var response = await _client.GetAsync($"/api/urls/{createdUrl!.ShortCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<ShortUrlResponse>();
        content.Should().NotBeNull();
        content!.OriginalUrl.Should().Be("https://example.com");
    }

    [Test]
    [DisplayName("Given_NonExistentCode_When_GetUrlDetails_Then_Returns_NotFound")]
    public async Task Given_NonExistentCode_When_GetUrlDetails_Then_Returns_NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/urls/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}