using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Url_shortener.Tests;

public class DatabaseConnectionTests
{
    /// <summary>
    /// Проверяет, что приложение может установить подключение к MySQL.
    /// Требует запущенный MySQL и корректную строку в appsettings.json или ConnectionStrings__DefaultConnection.
    /// </summary>
    [Fact]
    public async Task CanConnectToDatabase_ReturnsTrue()
    {
        var options = TestDbHelper.GetOptions();
        await using var context = new UrlShortenerContext(options);
        var canConnect = await context.Database.CanConnectAsync();

        Assert.True(canConnect, "Не удалось подключиться к БД. Проверьте, что MySQL запущен и строка подключения верна.");
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
