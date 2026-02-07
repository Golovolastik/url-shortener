using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Ensures UrlEditor adds https when missing so redirects work and we don’t store ambiguous scheme-less URLs.
/// </summary>
public class UrlEditorTests
{
    [Fact]
    public void ModifyUrl_WithHttp_ReturnsUnchanged()
    {
        var url = "http://example.com";
        var result = UrlEditor.ModifyUrl(url);
        Assert.Equal("http://example.com", result);
    }

    [Fact]
    public void ModifyUrl_WithHttps_ReturnsUnchanged()
    {
        var url = "https://example.com/path";
        var result = UrlEditor.ModifyUrl(url);
        Assert.Equal("https://example.com/path", result);
    }

    [Fact]
    public void ModifyUrl_WithoutScheme_AddsHttps()
    {
        var url = "example.com";
        var result = UrlEditor.ModifyUrl(url);
        Assert.Equal("https://example.com", result);
    }

    [Fact]
    public void ModifyUrl_WithoutScheme_WithPath_AddsHttps()
    {
        var url = "example.com/foo/bar?q=1";
        var result = UrlEditor.ModifyUrl(url);
        Assert.Equal("https://example.com/foo/bar?q=1", result);
    }

    [Theory]
    [InlineData("http://a.b")]
    [InlineData("https://a.b")]
    public void ModifyUrl_WithScheme_DoesNotDuplicateScheme(string url)
    {
        var result = UrlEditor.ModifyUrl(url);
        Assert.Equal(url, result);
    }
}
