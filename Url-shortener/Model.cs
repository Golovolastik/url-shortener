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
            entity.Property(e => e.Id).ValueGeneratedNever();
            // Unique ShortUrl so redirect /short/{code} always resolves to exactly one target.
            entity.HasIndex(e => e.ShortUrl).IsUnique();
            // Non-unique index on first 255 chars of LongUrl to speed up lookups; full uniqueness enforced in ShortenedUrlService.
            entity.HasIndex(e => e.LongUrl).HasDatabaseName("IX_ShortenedUrls_LongUrl");
        });
    }
}

public class ShortenedUrl
{
    /// <summary>Primary key is the same uid used to build ShortUrl (base62), so Id is derivable from the short code.</summary>
    public ulong Id { get; set; }
    public string LongUrl { get; set; }
    public string ShortUrl { get; set; }
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
}