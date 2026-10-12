using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DevDigest.Web.Dtos;
using Xunit;

namespace DevDigest.Web.Tests.Integration;

public class ArticlesApiTests
{
    [Fact]
    public async Task GetArticle_WhenArticleExists_ReturnsOkWithCorrectJson()
    {
        using var factory = new ArticlesApiFactory();
        using var client = factory.CreateClient();
        await factory.SeedArticleAsync();

        using var response = await client.GetAsync("/api/articles/123");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var article = await response.Content.ReadFromJsonAsync<ArticleResponse>();
        Assert.NotNull(article);
        TestArticles.AssertResponse(article);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(123, json.RootElement.GetProperty("id").GetInt32());
        Assert.False(json.RootElement.TryGetProperty("createdAt", out _));
        Assert.False(json.RootElement.TryGetProperty("isAiProcessed", out _));
    }

    [Fact]
    public async Task GetArticle_WhenArticleDoesNotExist_ReturnsNotFound()
    {
        using var factory = new ArticlesApiFactory();
        using var client = factory.CreateClient();
        await factory.SeedArticleAsync();

        using var response = await client.GetAsync("/api/articles/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
