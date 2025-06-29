using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;
using Company.Platform.UrlShortener.Contracts;
using Company.Platform.UrlShortener.App;

namespace Company.Platform.UrlShortener.ApiTests;

public class RedirectControllerTests : IDisposable
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    public RedirectControllerTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();

        // Disable automatic redirect following
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Test]
    [DisplayName("Given_ValidShortCode_When_Redirect_Then_Returns_PermanentRedirect")]
    public async Task Given_ValidShortCode_When_Redirect_Then_Returns_PermanentRedirect()
    {
        // Arrange - Create a URL first
        var createClient = _factory.CreateClient(); // Use auto-redirect client for setup
        var createRequest = new CreateShortUrlRequest("https://example.com", null);
        var createResponse = await createClient.PostAsJsonAsync("/api/shorten", createRequest);
        var createdUrl = await createResponse.Content.ReadFromJsonAsync<ShortUrlResponse>();

        // Act
        var response = await _client.GetAsync($"/{createdUrl!.ShortCode}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.MovedPermanently);
        response.Headers.Location?.ToString().Should().Be("https://example.com");
    }

    [Test]
    [DisplayName("Given_NonExistentCode_When_Redirect_Then_Returns_NotFound")]
    public async Task Given_NonExistentCode_When_Redirect_Then_Returns_NotFound()
    {
        // Act
        var response = await _client.GetAsync("/nonexistent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}