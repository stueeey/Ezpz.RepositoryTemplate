using Company.Platform.UrlShortener.App.Services;
using FluentAssertions;  // AwesomeAssertions uses FluentAssertions namespace
using TUnit.Core;

namespace Company.Platform.UrlShortener.App.Tests.Services;

public class ShortCodeGeneratorTests
{
    private readonly ShortCodeGenerator _generator = new();

    [Test]
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
    [Arguments(1, "aaaaab")]
    [Arguments(62, "aaaaba")]
    [Arguments(1000000, "aaemjc")]
    [DisplayName("Given_KnownId_When_GenerateShortCode_Then_Returns_DeterministicResult")]
    public void Given_KnownId_When_GenerateShortCode_Then_Returns_DeterministicResult(int id, string expected)
    {
        // Act
        var result = _generator.GenerateShortCode(id);
        
        // Assert
        result.Should().Be(expected);
    }
    
    [Test]
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