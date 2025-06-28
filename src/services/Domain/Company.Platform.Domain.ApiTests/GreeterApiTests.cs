using Company.Platform.Domain.App;
using FluentAssertions;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace Company.Platform.Domain.ApiTests;

[TestFixture]
public class GreeterApiTests
{
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new WebApplicationFactory<Program>();
    }

    [SetUp]
    public void SetUp()
    {
        var httpClient = _factory.WithWebHostBuilder(builder =>
        {
            // Add any test-specific configuration here
        }).CreateClient();

        _channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        _client = new Greeter.GreeterClient(_channel);
    }

    [TearDown]
    public void TearDown()
    {
        _channel?.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _factory?.Dispose();
    }

    private WebApplicationFactory<Program> _factory;
    private GrpcChannel _channel;
    private Greeter.GreeterClient _client;

    [Test]
    public async Task SayHello_WithValidName_ReturnsExpectedGreeting()
    {
        // Arrange
        var request = new HelloRequest { Name = "Integration Test" };

        // Act
        var response = await _client.SayHelloAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().Be("Hello Integration Test");
    }

    [Test]
    [TestCase("Alice")]
    [TestCase("Bob")]
    [TestCase("Charlie")]
    public async Task SayHello_WithDifferentNames_ReturnsCorrectGreeting(string name)
    {
        // Arrange
        var request = new HelloRequest { Name = name };

        // Act
        var response = await _client.SayHelloAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().Be($"Hello {name}");
    }

    [Test]
    public async Task SayHello_WithEmptyName_ReturnsGreeting()
    {
        // Arrange
        var request = new HelloRequest { Name = string.Empty };

        // Act
        var response = await _client.SayHelloAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().Be("Hello ");
    }
}