namespace Company.Platform.UrlShortener.App.Services;

public interface IShortCodeGenerator
{
    string GenerateShortCode(int id);
    int? DecodeShortCode(string shortCode);
}