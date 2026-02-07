using Microsoft.EntityFrameworkCore;

/// <summary>
/// Centralizes create-or-get logic so the same long URL never gets multiple short codes and collisions are retried.
/// </summary>
public static class ShortenedUrlService
{
    /// <summary>
    /// Returns existing row when possible to avoid duplicate short links; generates and retries on ShortUrl collision so DB stays consistent.
    /// </summary>
    public static async Task<ShortenedUrl?> CreateOrGetExistingAsync(UrlShortenerContext context, string? longUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(longUrl))
            return null;

        string normalizedLongUrl = UrlEditor.ModifyUrl(longUrl.Trim());

        var existing = await context.ShortenedUrls
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.LongUrl == normalizedLongUrl, cancellationToken);
        if (existing != null)
            return existing;

        ulong uid;
        string shortUrl;
        while (true)
        {
            uid = UidGenerator.GenerateUid();
            shortUrl = ShortUrlGenerator.GenerateShortUrl(uid);
            if (string.IsNullOrEmpty(shortUrl))
                shortUrl = "0";

            bool exists = await context.ShortenedUrls.AnyAsync(e => e.ShortUrl == shortUrl, cancellationToken);
            if (!exists)
                break;
        }

        var entity = new ShortenedUrl
        {
            Id = uid,
            LongUrl = normalizedLongUrl,
            ShortUrl = shortUrl,
            ClickCount = 0,
            CreatedAt = DateTime.UtcNow
        };
        context.ShortenedUrls.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }
}
