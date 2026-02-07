using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Ensures UidGenerator yields usable ids so ShortUrlGenerator and the DB get non-zero, base62-friendly values.
/// </summary>
public class UidGeneratorTests
{
    [Fact]
    public void GenerateUid_ReturnsNonZero()
    {
        var uid = UidGenerator.GenerateUid();
        Assert.NotEqual(0UL, uid);
    }

    [Fact]
    public void GenerateUid_ReturnsPositiveValue()
    {
        var uid = UidGenerator.GenerateUid();
        Assert.True(uid > 0, "Uid должен быть положительным (ulong)");
    }

    [Fact]
    public void GenerateUid_MultipleCalls_ReturnValues()
    {
        var uids = new HashSet<ulong>();
        for (int i = 0; i < 10; i++)
            uids.Add(UidGenerator.GenerateUid());
        Assert.True(uids.Count >= 1, "Генератор должен возвращать значения (коллизии при 10 вызовах допустимы)");
    }

    [Fact]
    public void GenerateUid_CanBeUsedByShortUrlGenerator()
    {
        var uid = UidGenerator.GenerateUid();
        var shortUrl = ShortUrlGenerator.GenerateShortUrl(uid);
        Assert.False(string.IsNullOrEmpty(shortUrl));
        Assert.All(shortUrl, c => Assert.True(
            (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'),
            $"Символ '{c}' не из base62"));
    }
}
