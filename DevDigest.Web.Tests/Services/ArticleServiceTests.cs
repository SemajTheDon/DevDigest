using DevDigest.Data.Data;
using DevDigest.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DevDigest.Web.Tests.Services;

public class ArticleServiceTests
{
    [Fact]
    public async Task GetArticleAsync_WhenArticleExists_ReturnsExpectedResponse()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateContext(connection);
        await db.Database.EnsureCreatedAsync();
        db.Articles.Add(TestArticles.CreateEntity());
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        var service = new ArticleService(db);

        var result = await service.GetArticleAsync(123);

        Assert.NotNull(result);
        TestArticles.AssertResponse(result);
    }

    [Fact]
    public async Task GetArticleAsync_WhenArticleDoesNotExist_ReturnsNull()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var db = CreateContext(connection);
        await db.Database.EnsureCreatedAsync();
        db.Articles.Add(TestArticles.CreateEntity());
        await db.SaveChangesAsync();
        var service = new ArticleService(db);

        var result = await service.GetArticleAsync(999);

        Assert.Null(result);
    }

    private static AppDbContext CreateContext(SqliteConnection connection) =>
        new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
}
