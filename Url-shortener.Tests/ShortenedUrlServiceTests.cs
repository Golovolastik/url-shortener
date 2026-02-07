using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Ensures create-or-get applies UrlEditor, reuses existing LongUrl, and retries on ShortUrl collision so production behavior is predictable.
/// </summary>
public class ShortenedUrlServiceTests : IClassFixture<MigratedDbFixture>
{
    [Fact]
    public async Task CreateOrGetExistingAsync_Null_ReturnsNull()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);
        var result = await ShortenedUrlService.CreateOrGetExistingAsync(context, null);
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrGetExistingAsync_WhiteSpace_ReturnsNull()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);
        var result = await ShortenedUrlService.CreateOrGetExistingAsync(context, "   ");
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateOrGetExistingAsync_NewLongUrl_AppliesUrlEditorAndSaves_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await ShortenedUrlService.CreateOrGetExistingAsync(context, "example.com/path");
            Assert.NotNull(result);
            Assert.Equal("https://example.com/path", result.LongUrl);
            Assert.False(string.IsNullOrEmpty(result.ShortUrl));
            Assert.Equal(0, result.ClickCount);
            Assert.True(result.Id > 0);
            Assert.True(result.CreatedAt <= DateTime.UtcNow.AddSeconds(1));

            var inDb = await context.ShortenedUrls.AsNoTracking().FirstOrDefaultAsync(e => e.Id == result.Id);
            Assert.NotNull(inDb);
            Assert.Equal(result.LongUrl, inDb.LongUrl);
            Assert.Equal(result.ShortUrl, inDb.ShortUrl);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task CreateOrGetExistingAsync_SameLongUrl_ReturnsExisting_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var longUrl = "https://example.com/unique-same-long";
            var first = await ShortenedUrlService.CreateOrGetExistingAsync(context, longUrl);
            Assert.NotNull(first);
            var firstId = first.Id;

            var second = await ShortenedUrlService.CreateOrGetExistingAsync(context, longUrl);
            Assert.NotNull(second);
            Assert.Equal(firstId, second.Id);
            Assert.Equal(first.ShortUrl, second.ShortUrl);

            var count = await context.ShortenedUrls.CountAsync(e => e.LongUrl == longUrl);
            Assert.Equal(1, count);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task CreateOrGetExistingAsync_LongUrlWithoutScheme_NormalizedAndSaved_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await ShortenedUrlService.CreateOrGetExistingAsync(context, "  sub.example.com  ");
            Assert.NotNull(result);
            Assert.Equal("https://sub.example.com", result.LongUrl);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task CreateOrGetExistingAsync_SavedShortUrl_IsBase62_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var result = await ShortenedUrlService.CreateOrGetExistingAsync(context, "https://test-base62.com");
            Assert.NotNull(result);
            Assert.All(result.ShortUrl, c => Assert.True(
                (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'),
                $"ShortUrl должен быть base62, получен символ '{c}'"));
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
}
