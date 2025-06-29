# Testing Guidelines

This document outlines the testing standards and conventions for this repository.

## Testing Framework Standards

### **Primary Testing Stack (Recommended)**

- **Test Framework**: [TUnit](https://github.com/thomhurst/TUnit) - Modern, fast, and feature-rich testing framework
- **Assertions**: [AwesomeAssertions](https://github.com/awesome-assertions/AwesomeAssertions) - Fluent assertion library (FluentAssertions fork)
- **Mocking**: [FakeItEasy](https://fakeiteasy.github.io/) - Simple and intuitive mocking framework
- **E2E Testing**: [Playwright](https://playwright.dev/dotnet/) - Cross-browser web testing
## Project Structure

Follow these naming conventions for test projects:

```
src/services/{ServiceName}/
├── {ServiceName}.Tests/            # Unit tests (TUnit)
├── {ServiceName}.ApiTests/         # Integration tests (TUnit)
└── {ServiceName}.E2ETests/         # End-to-end tests (TUnit + Playwright)
```

## Test Categories

Use `[Category]` attributes to organize tests for selective execution:

- `[Category("Unit")]` - Fast, isolated unit tests
- `[Category("Integration")]` - API/database integration tests  
- `[Category("E2E")]` - Full end-to-end browser tests

## Test Naming Conventions

### Test Methods
Use the **Given_Subject_Expectation pattern with descriptive names:

```csharp
[Test]
[DisplayName("Given a valid url, CreateShortUrl should return 201 Created")]
public async Task Given_ValidUrl_CreateShortUrl_Should_Return_Created()
```

### Test Classes
End test class names with `Tests`:

```csharp
[TestClass]
public class UrlShortenerServiceTests
```

## TUnit Test Example

```csharp
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using FakeItEasy;
using TUnit.Core;

[TestClass]
public class UserServiceTests
{
    [Test]
    [Category("Unit")]
    [DisplayName("Given_ValidUser_When_CreateUser_Then_Returns_UserId")]
    public async Task Given_ValidUser_When_CreateUser_Then_Returns_UserId()
    {
        // Arrange
        var mockRepository = A.Fake<IUserRepository>();
        var service = new UserService(mockRepository);
        var user = new User { Name = "John Doe", Email = "john@example.com" };
        
        A.CallTo(() => mockRepository.CreateAsync(user))
            .Returns(Task.FromResult(123));

        // Act
        var result = await service.CreateUserAsync(user);

        // Assert
        result.Should().Be(123);
        A.CallTo(() => mockRepository.CreateAsync(user))
            .MustHaveHappenedOnceExactly();
    }
    
    [Test]
    [TestCase("", "john@example.com")]
    [TestCase("John", "")]
    [TestCase(null, "john@example.com")]
    [Category("Unit")]
    [DisplayName("Given_InvalidUser_When_CreateUser_Then_Throws_ValidationException")]
    public async Task Given_InvalidUser_When_CreateUser_Then_Throws_ValidationException(
        string name, string email)
    {
        // Arrange
        var mockRepository = A.Fake<IUserRepository>();
        var service = new UserService(mockRepository);
        var user = new User { Name = name, Email = email };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.CreateUserAsync(user));
    }
}
```

## Integration Test Example

```csharp
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using Microsoft.AspNetCore.Mvc.Testing;
using TUnit.Core;

[TestClass]
public class UsersControllerTests : IDisposable
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
    [DisplayName("Given_ValidUser_When_PostUser_Then_Returns_Created")]
    public async Task Given_ValidUser_When_PostUser_Then_Returns_Created()
    {
        // Arrange
        var user = new { Name = "John Doe", Email = "john@example.com" };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/users", user);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
    
    public void Dispose() => TearDown();
}
```

## E2E Test Example

```csharp
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using Microsoft.Playwright;
using TUnit.Core;

[TestClass]
public class UserWorkflowE2ETests : IAsyncDisposable
{
    private IBrowser _browser;
    private IPage _page;
    
    [SetUp]
    public async Task Setup()
    {
        var playwright = await Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _page = await _browser.NewPageAsync();
    }
    
    [Test]
    [Category("E2E")]
    [DisplayName("Given_UserRegistration_When_CompleteFlow_Then_UserIsCreated")]
    public async Task Given_UserRegistration_When_CompleteFlow_Then_UserIsCreated()
    {
        // Navigate and interact with UI
        await _page.GotoAsync("http://localhost:5000/register");
        await _page.FillAsync("#name", "John Doe");
        await _page.FillAsync("#email", "john@example.com");
        await _page.ClickAsync("#submit");
        
        // Assert
        var successMessage = await _page.TextContentAsync("#success");
        successMessage.Should().Contain("User created successfully");
    }
    
    [TearDown]
    public async Task TearDown()
    {
        if (_page != null) await _page.CloseAsync();
        if (_browser != null) await _browser.CloseAsync();
    }
    
    public async ValueTask DisposeAsync() => await TearDown();
}
```

## Test Project Configuration

### Unit Test Project (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="TUnit" />
    <PackageReference Include="TUnit.Assertions" />
    <PackageReference Include="AwesomeAssertions" />
    <PackageReference Include="FakeItEasy" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" />
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\{ServiceName}.App\{ServiceName}.App.csproj" />
  </ItemGroup>
</Project>
```

### E2E Test Project (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="TUnit" />
    <PackageReference Include="TUnit.Assertions" />
    <PackageReference Include="AwesomeAssertions" />
    <PackageReference Include="Microsoft.Playwright" />
    <PackageReference Include="coverlet.collector" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\{ServiceName}.Client\{ServiceName}.Client.csproj" />
  </ItemGroup>
</Project>
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run tests by category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=E2E"

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Best Practices

### Unit Tests
- ✅ Fast execution (< 1ms per test)
- ✅ No external dependencies (database, network, file system)
- ✅ Use in-memory databases for EF Core tests
- ✅ Mock all external services with FakeItEasy
- ✅ Test one logical unit per test method

### Integration Tests
- ✅ Test API endpoints end-to-end
- ✅ Use WebApplicationFactory for ASP.NET Core
- ✅ Test authentication/authorization scenarios
- ✅ Validate request/response models

### E2E Tests
- ✅ Test critical user workflows
- ✅ Keep scenarios focused and independent
- ✅ Use Page Object Model for complex UIs
- ✅ Include cross-browser testing for public applications

### General Guidelines
- ✅ Write tests before or alongside implementation
- ✅ Use descriptive test names that explain the scenario
- ✅ Follow AAA pattern (Arrange, Act, Assert)
- ✅ Keep tests independent and deterministic
- ✅ Use test categories for organized execution
- ✅ Aim for higher test coverage on business logic

**Note**: AwesomeAssertions uses the same `FluentAssertions` namespace and identical API, making migration seamless.

## Reference Links

- [TUnit Documentation](https://github.com/thomhurst/TUnit)
- [AwesomeAssertions Documentation](https://github.com/awesome-assertions/AwesomeAssertions)
- [FakeItEasy Documentation](https://fakeiteasy.github.io/)
- [Playwright .NET Documentation](https://playwright.dev/dotnet/)