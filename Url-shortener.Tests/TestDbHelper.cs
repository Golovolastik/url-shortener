using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.EntityFrameworkCore.Extensions;

namespace Url_shortener.Tests;

public static class TestDbHelper
{
    public static string GetConnectionString()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
        return config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection не задана.");
    }

    public static DbContextOptions<UrlShortenerContext> GetOptions()
    {
        var connectionString = GetConnectionString();
        return new DbContextOptionsBuilder<UrlShortenerContext>()
            .UseMySQL(connectionString)
            .Options;
    }
}
