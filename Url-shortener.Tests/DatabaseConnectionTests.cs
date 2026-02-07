using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Url_shortener.Tests;

public class DatabaseConnectionTests
{
    /// <summary>
    /// Fails fast in CI/local if MySQL is down or connection string is wrong so DB-dependent tests aren’t misleading.
    /// </summary>
    [Fact]
    public async Task CanConnectToDatabase_ReturnsTrue()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);
        var canConnect = await context.Database.CanConnectAsync();

        Assert.True(canConnect, "Could not connect to DB. Ensure MySQL is running and connection string is correct.");
    }

    [Fact]
    public void Database_ProviderName_IsMySql()
    {
        var options = TestDbHelper.GetOptions();
        using var context = new UrlShortenerContext(options);
        var providerName = context.Database.ProviderName;

        Assert.Contains("MySQL", providerName ?? "", StringComparison.OrdinalIgnoreCase);
    }
}
