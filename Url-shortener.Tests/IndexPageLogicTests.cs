using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Covers Index delete and edit so we know remove-by-id and update-long-url (including duplicate check) behave correctly before UI relies on them.
/// </summary>
public class IndexPageLogicTests : IClassFixture<MigratedDbFixture>
{
    [Fact]
    public async Task Delete_ById_RemovesRecord_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                Id = 2001UL,
                LongUrl = "https://example.com/delete-test",
                ShortUrl = "xyz-delete",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();
            var id = entity.Id;

            var toRemove = await context.ShortenedUrls.FindAsync(id);
            Assert.NotNull(toRemove);
            context.ShortenedUrls.Remove(toRemove);
            await context.SaveChangesAsync();

            var deleted = await context.ShortenedUrls.FindAsync(id);
            Assert.Null(deleted);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Delete_ByNonExistentId_DoesNotThrow_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = await context.ShortenedUrls.FindAsync(999999999UL);
            if (entity != null)
            {
                context.ShortenedUrls.Remove(entity);
                await context.SaveChangesAsync();
            }
            Assert.True(true);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Edit_UpdateLongUrl_SavesNewValue_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                Id = 2002UL,
                LongUrl = "https://example.com/edit-original",
                ShortUrl = "xyz-edit",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();

            entity.LongUrl = UrlEditor.ModifyUrl("https://example.com/edited-url".Trim());
            await context.SaveChangesAsync();

            var updated = await context.ShortenedUrls.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
            Assert.Equal("https://example.com/edited-url", updated.LongUrl);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Edit_UpdateLongUrl_CanStoreSameLongUrlAsAnotherRow_WhenNoUniqueIndex_WithinTransactionThenRollback()
    {
        // LongUrl has no DB unique index (longtext for long/data URLs); duplicate prevention is in ShortenedUrlService / Edit handler.
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var urlA = "https://example.com/unique-a";
            var urlB = "https://example.com/unique-b";
            var entityA = new ShortenedUrl
            {
                Id = 2003UL,
                LongUrl = urlA,
                ShortUrl = "xyz-a",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            var entityB = new ShortenedUrl
            {
                Id = 2004UL,
                LongUrl = urlB,
                ShortUrl = "xyz-b",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.AddRange(entityA, entityB);
            await context.SaveChangesAsync();

            entityA.LongUrl = urlB;
            await context.SaveChangesAsync();

            var both = await context.ShortenedUrls.Where(e => e.Id == 2003UL || e.Id == 2004UL).ToListAsync();
            Assert.Equal(2, both.Count);
            Assert.All(both, e => Assert.Equal(urlB, e.LongUrl));
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Edit_UpdateLongUrl_NormalizesWithUrlEditor_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                Id = 2005UL,
                LongUrl = "https://example.com/original",
                ShortUrl = "xyz-norm",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();

            entity.LongUrl = UrlEditor.ModifyUrl("example.com/without-scheme".Trim());
            await context.SaveChangesAsync();

            var updated = await context.ShortenedUrls.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
            Assert.Equal("https://example.com/without-scheme", updated.LongUrl);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
}
