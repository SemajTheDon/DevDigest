using DevDigest.Data.Models;
using DevDigest.Web.Dtos;
using Xunit;

namespace DevDigest.Web.Tests;

internal static class TestArticles
{
    public static Article CreateEntity() => new()
    {
        Id = 123,
        Title = "Building a developer digest",
        Url = "https://example.com/digest",
        Source = ".NET Blog",
        PublishedAt = new DateTime(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc),
        Summary = "Original feed summary",
        Category = ".NET",
        AiSummary = "Generated article summary",
        KeyTakeaways = "- Use DTOs\n- Test your API",
        IsAiProcessed = true
    };

    public static ArticleResponse CreateResponse() => new()
    {
        Id = 123,
        Title = "Building a developer digest",
        Url = "https://example.com/digest",
        Source = ".NET Blog",
        PublishedAt = new DateTime(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc),
        Summary = "Original feed summary",
        Category = ".NET",
        AiSummary = "Generated article summary",
        KeyTakeaways = "- Use DTOs\n- Test your API"
    };

    public static void AssertResponse(ArticleResponse actual)
    {
        var expected = CreateResponse();
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Url, actual.Url);
        Assert.Equal(expected.Source, actual.Source);
        Assert.Equal(expected.PublishedAt, actual.PublishedAt);
        Assert.Equal(expected.Summary, actual.Summary);
        Assert.Equal(expected.Category, actual.Category);
        Assert.Equal(expected.AiSummary, actual.AiSummary);
        Assert.Equal(expected.KeyTakeaways, actual.KeyTakeaways);
    }
}
