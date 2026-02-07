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

    [Fact]
    public void IsValid_UrlWithQueryParamsAndPercentEncoding_ReturnsTrue()
    {
        var url = "https://catalog.onliner.by/?utm_source=google&utm_medium=cpc&utm_content=onliner%20by%20catalog&utm_content={source}&gclid=abc123";
        var result = UrlValidator.IsValid(url, out var error);
        Assert.True(result);
        Assert.Null(error);
    }

    [Fact]
    public void IsValid_UrlWithCurlyBracesInQuery_ReturnsTrue()
    {
        var result = UrlValidator.IsValid("https://example.com/?tag={source}", out var error);
        Assert.True(result);
        Assert.Null(error);
    }

    [Fact]
    public void IsValid_DataUrl_Base64Image_ReturnsTrue()
    {
        var dataUrl = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBwoJCAgJCgoLCwgICAoJCAgICBsICQoNIB0iIiAdHx8kKDQsJCYxJx8fLT0tMTU3Ojo6Iys/RDY4QzQ5OjcBCgoKDg0OGhAQGy0mICYtLTEvLjArNy0tKysrLisrKy0rLSstLS0rKystKystLSstLSstLS0tNS0tLTEtLSsvLf/AABEIAKgBLAMBIgACEQEDEQH/xAAbAAAABwEAAAAAAAAAAAAAAAAAAQIDBAUGB//EAEkQAAIBAwICBgYGAQUH";
        var result = UrlValidator.IsValid(dataUrl, out var error);
        Assert.True(result);
        Assert.Null(error);
    }

    [Fact]
    public void IsValid_DataUrl_PlainText_ReturnsTrue()
    {
        var result = UrlValidator.IsValid("data:text/plain,Hello%20World", out var error);
        Assert.True(result);
        Assert.Null(error);
    }

    [Fact]
    public void IsValid_DataUrl_NoComma_ReturnsFalse()
    {
        var result = UrlValidator.IsValid("data:image/png", out var error);
        Assert.False(result);
        Assert.NotNull(error);
        Assert.Contains("data URL", error!);
    }

    [Fact]
    public void IsValid_DataUrl_EmptyData_ReturnsFalse()
    {
        var result = UrlValidator.IsValid("data:image/jpeg;base64,", out var error);
        Assert.False(result);
        Assert.NotNull(error);
    }
}
