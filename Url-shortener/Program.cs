using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UrlShortenerContext>(options =>
    options.UseMySQL(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UrlShortenerContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Redirect short.yankovich.by/{shortUrl} to long URL; exclude Razor Page paths (Index, Privacy, Error).
app.MapGet("/{shortUrl:regex(^(?!Index$|Privacy$|Error$).+)}", async (string shortUrl, UrlShortenerContext db) =>
{
    var entry = await db.ShortenedUrls.FirstOrDefaultAsync(e => e.ShortUrl == shortUrl);
    if (entry == null)
        return Results.NotFound();

    entry.ClickCount++;
    await db.SaveChangesAsync();

    return Results.Redirect(entry.LongUrl, permanent: true, preserveMethod: false);
});

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
