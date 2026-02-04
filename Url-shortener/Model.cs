using Microsoft.EntityFrameworkCore;

public class UrlShortenerContext : DbContext
{
    public UrlShortenerContext(DbContextOptions<UrlShortenerContext> options)
        : base(options)
    {
    }

    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
}

public class ShortenedUrl
{
    public int Id { get; set; }
    public string LongUrl { get; set; }
    public string ShortUrl { get; set; }
    public int ClickCount { get; set; }
    public DateTime CreatedAt { get; set; }
}