using Microsoft.EntityFrameworkCore;

public class UrlShortenerContext : DbContext
{
    public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options)
        : base(options)
    {
    }

    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(entity =>
        {
            // Короткий код должен быть уникальным: один код — одна запись при редиректе.
            entity.HasIndex(e => e.ShortUrl).IsUnique();
            // Один длинный URL — одна короткая ссылка (повторное сокращение возвращает существующую).
            entity.HasIndex(e => e.LongUrl).IsUnique();
        });
    }
}

public class ShortenedUrl
{
    public int Id { get; set; }
    public string LongUrl { get; set; }
    public string ShortUrl { get; set; }
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
}