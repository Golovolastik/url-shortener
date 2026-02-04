using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Url_shortener.Tests;

/// <summary>
/// Применяет миграции к БД один раз перед первым тестом в ShortenedUrlCrudTests.
/// </summary>
public class MigratedDbFixture : IDisposable
{
    public MigratedDbFixture()
    {
        var options = TestDbHelper.GetOptions();
        using var context = new UrlShortenerContext(options);
        context.Database.Migrate();
    }

    public void Dispose() => GC.SuppressFinalize(this);
}

/// <summary>
/// CRUD-тесты для ShortenedUrl. Все операции выполняются в транзакции с откатом,
/// чтобы данные в БД не изменялись после тестов. Схема БД создаётся фикстурой один раз.
/// </summary>
public class ShortenedUrlCrudTests : IClassFixture<MigratedDbFixture>
{
    [Fact]
    public async Task Create_AddsEntity_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                LongUrl = "https://example.com/crud-create-test",
                ShortUrl = "abc-create",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();

            Assert.True(entity.Id > 0);

            var found = await context.ShortenedUrls.FindAsync(entity.Id);
            Assert.NotNull(found);
            Assert.Equal(entity.LongUrl, found.LongUrl);
            Assert.Equal(entity.ShortUrl, found.ShortUrl);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Read_GetById_ReturnsEntity_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                LongUrl = "https://example.com/crud-read-test",
                ShortUrl = "abc-read",
                ClickCount = 5,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();

            var read = await context.ShortenedUrls.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.Id);
            Assert.NotNull(read);
            Assert.Equal(entity.LongUrl, read.LongUrl);
            Assert.Equal(entity.ShortUrl, read.ShortUrl);
            Assert.Equal(5, read.ClickCount);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Update_ModifiesEntity_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                LongUrl = "https://example.com/crud-update-test",
                ShortUrl = "abc-update",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();

            entity.ClickCount = 10;
            entity.LongUrl = "https://example.com/updated";
            await context.SaveChangesAsync();

            var updated = await context.ShortenedUrls.AsNoTracking().FirstAsync(e => e.Id == entity.Id);
            Assert.Equal(10, updated.ClickCount);
            Assert.Equal("https://example.com/updated", updated.LongUrl);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }

    [Fact]
    public async Task Delete_RemovesEntity_WithinTransactionThenRollback()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var entity = new ShortenedUrl
            {
                LongUrl = "https://example.com/crud-delete-test",
                ShortUrl = "abc-delete",
                ClickCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            context.ShortenedUrls.Add(entity);
            await context.SaveChangesAsync();
            var id = entity.Id;

            context.ShortenedUrls.Remove(entity);
            await context.SaveChangesAsync();

            var deleted = await context.ShortenedUrls.FindAsync(id);
            Assert.Null(deleted);
        }
        finally
        {
            await transaction.RollbackAsync();
        }
    }
}
