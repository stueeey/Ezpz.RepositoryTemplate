using System.Text;

namespace Company.Platform.UrlShortener.App.Services;

public class ShortCodeGenerator : IShortCodeGenerator
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int MinLength = 6;

    public string GenerateShortCode(int id)
    {
        var result = new StringBuilder();
        var value = id;

        while (value > 0)
        {
            result.Insert(0, Alphabet[value % Alphabet.Length]);
            value /= Alphabet.Length;
        }

        // Pad to minimum length
        while (result.Length < MinLength)
        {
            result.Insert(0, Alphabet[0]);
        }

        return result.ToString();
    }

    public int? DecodeShortCode(string shortCode)
    {
        if (string.IsNullOrEmpty(shortCode))
            return null;

        int result = 0;
        int multiplier = 1;

        for (int i = shortCode.Length - 1; i >= 0; i--)
        {
            int charIndex = Alphabet.IndexOf(shortCode[i]);
            if (charIndex == -1)
                return null; // Invalid character

            result += charIndex * multiplier;
            multiplier *= Alphabet.Length;
        }

        return result;
    }
}