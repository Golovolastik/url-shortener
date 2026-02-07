using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Ensures base62 encoding is deterministic and URL-safe so short codes are stable and usable in paths.
/// </summary>
public class ShortUrlGeneratorTests
{
    [Fact]
    public void GenerateShortUrl_Zero_ReturnsEmptyString()
    {
        var result = ShortUrlGenerator.GenerateShortUrl(0);
        Assert.Equal("", result);
    }

    [Fact]
    public void GenerateShortUrl_SingleDigitBase62_ReturnsOneCharacter()
    {
        Assert.Equal("1", ShortUrlGenerator.GenerateShortUrl(1));
        Assert.Equal("9", ShortUrlGenerator.GenerateShortUrl(9));
        Assert.Equal("a", ShortUrlGenerator.GenerateShortUrl(10));
        Assert.Equal("z", ShortUrlGenerator.GenerateShortUrl(35));
        Assert.Equal("A", ShortUrlGenerator.GenerateShortUrl(36));
        Assert.Equal("Z", ShortUrlGenerator.GenerateShortUrl(61));
    }

    [Fact]
    public void GenerateShortUrl_62_ReturnsBase62_10()
    {
        var result = ShortUrlGenerator.GenerateShortUrl(62);
        Assert.Equal("10", result);
    }

    [Fact]
    public void GenerateShortUrl_3844_ReturnsBase62_100()
    {
        // 62 * 62 = 3844
        var result = ShortUrlGenerator.GenerateShortUrl(3844);
        Assert.Equal("100", result);
    }

    [Fact]
    public void GenerateShortUrl_LargeUid_ReturnsValidBase62String()
    {
        var result = ShortUrlGenerator.GenerateShortUrl(ulong.MaxValue);
        Assert.False(string.IsNullOrEmpty(result));
        Assert.All(result, c => Assert.True(
            (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'),
            $"Character '{c}' is not valid base62"));
    }

    [Fact]
    public void GenerateShortUrl_IsDeterministic()
    {
        var uid = 12345UL;
        var a = ShortUrlGenerator.GenerateShortUrl(uid);
        var b = ShortUrlGenerator.GenerateShortUrl(uid);
        Assert.Equal(a, b);
    }

    [Theory]
    [InlineData(0UL, "")]
    [InlineData(1UL, "1")]
    [InlineData(62UL, "10")]
    [InlineData(63UL, "11")]
    [InlineData(3844UL, "100")]
    [InlineData(238328UL, "1000")]
    public void GenerateShortUrl_KnownValues_MatchesExpected(ulong uid, string expected)
    {
        var result = ShortUrlGenerator.GenerateShortUrl(uid);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateShortUrl_DifferentUids_ProduceDifferentStrings()
    {
        var r1 = ShortUrlGenerator.GenerateShortUrl(1);
        var r2 = ShortUrlGenerator.GenerateShortUrl(2);
        var r62 = ShortUrlGenerator.GenerateShortUrl(62);
        var r63 = ShortUrlGenerator.GenerateShortUrl(63);
        Assert.NotEqual(r1, r2);
        Assert.NotEqual(r2, r62);
        Assert.NotEqual(r62, r63);
    }
}
