using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Company.Platform.Domain.E2ETests;

[TestFixture]
[Parallelizable(ParallelScope.Self)]
public class GreeterE2ETests : PageTest
{
    private string _baseUrl;

    [SetUp]
    public void SetUp()
    {
        // In a real scenario, this would point to the deployed service
        // For now, this assumes the service is running locally
        _baseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "https://localhost:5001";
    }

    [Test]
    public async Task HomePage_ShouldDisplayGrpcMessage()
    {
        // Navigate to the home page
        await Page.GotoAsync(_baseUrl);

        // Check that the gRPC message is displayed
        var content = await Page.TextContentAsync("body");
        content.Should().Contain("Communication with gRPC endpoints must be made through a gRPC client");
    }

    [Test]
    public async Task GrpcEndpoint_ShouldBeAccessible()
    {
        // This test verifies that the gRPC endpoint is accessible
        // In a real E2E test, you might use a gRPC client to make actual calls
        
        var response = await Page.APIRequest.GetAsync(_baseUrl);
        response.Status.Should().Be(200);
    }

    [Test]
    [Category("Smoke")]
    public async Task Service_ShouldBeHealthy()
    {
        // In a real scenario, you might have a health check endpoint
        // For now, we just verify the service is responding
        
        var response = await Page.APIRequest.GetAsync(_baseUrl);
        response.Ok.Should().BeTrue();
    }
}