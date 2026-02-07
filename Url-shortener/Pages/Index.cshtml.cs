using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Url_shortener.Pages;

public class IndexModel : PageModel
{
    private readonly UrlShortenerContext _context;

    public IndexModel(UrlShortenerContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string? LongUrl { get; set; }

    /// <summary>Shown under the main form so the user knows why submit was rejected without losing the table.</summary>
    public string? ValidationError { get; set; }

    public IList<ShortenedUrl> ShortenedUrls { get; set; } = new List<ShortenedUrl>();

    // Bound from the edit modal so we can post back with handler "Edit" and preserve Id/LongUrl on validation failure.
    [BindProperty(Name = "EditId")]
    public ulong EditId { get; set; }
    [BindProperty(Name = "EditLongUrl")]
    public string? EditLongUrl { get; set; }
    /// <summary>Shown inside the edit popup so the user can correct the long URL without closing the modal.</summary>
    public string? EditValidationError { get; set; }

    public async Task OnGetAsync()
    {
        ShortenedUrls = await _context.ShortenedUrls
            .OrderByDescending(u => u.CreatedAt)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UrlValidator.IsValid(LongUrl, out var error))
        {
            ValidationError = error;
            ShortenedUrls = await _context.ShortenedUrls
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();
            return Page();
        }

        _ = await ShortenedUrlService.CreateOrGetExistingAsync(_context, LongUrl!);

        ShortenedUrls = await _context.ShortenedUrls
            .OrderByDescending(u => u.CreatedAt)
            .Take(10)
            .AsNoTracking()
            .ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
        if (!UrlValidator.IsValid(EditLongUrl, out var error))
        {
            EditValidationError = error;
            ShortenedUrls = await _context.ShortenedUrls
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();
            return Page();
        }

        var entity = await _context.ShortenedUrls.FindAsync(EditId);
        if (entity == null)
            return RedirectToPage();

        string newLongUrl = UrlEditor.ModifyUrl(EditLongUrl!.Trim());
        var duplicate = await _context.ShortenedUrls.FirstOrDefaultAsync(e => e.LongUrl == newLongUrl && e.Id != EditId);
        if (duplicate != null)
        {
            EditValidationError = "Такой длинный URL уже есть у другой ссылки.";
            ShortenedUrls = await _context.ShortenedUrls
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .AsNoTracking()
                .ToListAsync();
            return Page();
        }

        entity.LongUrl = newLongUrl;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(ulong DeleteId)
    {
        var entity = await _context.ShortenedUrls.FindAsync(DeleteId);
        if (entity != null)
        {
            _context.ShortenedUrls.Remove(entity);
            await _context.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}
