using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Ensures UrlValidator rejects bad input (no TLD, invalid chars) and accepts scheme-optional URLs so the form and edit modal stay safe.
/// </summary>
public class UrlValidatorTests
{
    [Theory]
    [InlineData("example.com")]
    [InlineData("https://example.com")]
    [InlineData("http://example.com")]
    [InlineData("sub.example.com")]
    [InlineData("https://sub.example.org/path")]
    [InlineData("example.co.uk")]
    public void IsValid_ValidUrl_ReturnsTrue(string url)
    {
        var result = UrlValidator.IsValid(url, out var error);
        Assert.True(result);
        Assert.Null(error);
    }

    [Fact]
    public void IsValid_WithoutScheme_ReturnsTrue()
    {
        var result = UrlValidator.IsValid("example.com", out var error);
        Assert.True(result);
        Assert.Null(error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValid_NullOrWhiteSpace_ReturnsFalse(string? input)
    {
        var result = UrlValidator.IsValid(input, out var error);
        Assert.False(result);
        Assert.NotNull(error);
        Assert.Contains("Введите", error!);
    }

    [Fact]
    public void IsValid_NoTld_ReturnsFalse()
    {
        var result = UrlValidator.IsValid("http://localhost", out var error);
        Assert.False(result);
        Assert.NotNull(error);
        Assert.Contains("домен", error!);
    }

    [Fact]
    public void IsValid_TldIsDigitsOnly_ReturnsFalse()
    {
        // TLD must be dot + letters so .123 is rejected and we don’t store non-URLs.
        var result = UrlValidator.IsValid("https://example.123", out var error);
        Assert.False(result);
        Assert.NotNull(error);
    }

    [Theory]
    [InlineData("https://example.com space")]
    [InlineData("https://example.com\t")]
    [InlineData("https://example.com<script>")]
    [InlineData("https://example.com\"")]
    [InlineData("https://example.com\\path")]
    public void IsValid_InvalidCharacters_ReturnsFalse(string url)
    {
        var result = UrlValidator.IsValid(url, out var error);
        Assert.False(result);
        Assert.NotNull(error);
        Assert.Contains("недопустимые символы", error!);
    }

    [Fact]
    public void IsValid_ValidWithPathAndQuery_ReturnsTrue()
    {
        var result = UrlValidator.IsValid("example.com/path?q=1", out var error);
        Assert.True(result);
        Assert.Null(error);
    }
}
