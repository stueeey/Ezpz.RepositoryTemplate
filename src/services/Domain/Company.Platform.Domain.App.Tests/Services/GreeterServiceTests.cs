using Company.Platform.Domain.App.Services;
using FluentAssertions;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Company.Platform.Domain.App.Tests.Services;

public class GreeterServiceTests
{
    private readonly Mock<ILogger<GreeterService>> _loggerMock;
    private readonly GreeterService _service;

    public GreeterServiceTests()
    {
        _loggerMock = new Mock<ILogger<GreeterService>>();
        _service = new GreeterService();
    }

    [Fact]
    public async Task SayHello_WithValidName_ReturnsGreeting()
    {
        // Arrange
        var request = new HelloRequest { Name = "World" };
        var context = Mock.Of<ServerCallContext>();

        // Act
        var response = await _service.SayHello(request, context);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().Be("Hello World");
    }

    [Fact]
    public async Task SayHello_WithEmptyName_ReturnsGreetingWithStranger()
    {
        // Arrange
        var request = new HelloRequest { Name = "" };
        var context = Mock.Of<ServerCallContext>();

        // Act
        var response = await _service.SayHello(request, context);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().Be("Hello ");
    }

    [Theory]
    [InlineData("Alice")]
    [InlineData("Bob")]
    [InlineData("Charlie")]
    public async Task SayHello_WithDifferentNames_ReturnsCorrectGreeting(string name)
    {
        // Arrange
        var request = new HelloRequest { Name = name };
        var context = Mock.Of<ServerCallContext>();

        // Act
        var response = await _service.SayHello(request, context);

        // Assert
        response.Should().NotBeNull();
        response.Message.Should().Be($"Hello {name}");
    }
}