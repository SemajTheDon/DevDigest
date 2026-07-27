using DevDigest.Data.Models;

namespace DevDigest.Web.Services;

public class AiSummaryService
{
    public Task ProcessArticleAsync(Article article)
    {
        article.AiSummary =  $"This article discusses {article.Title}. It highlights recent updates, explains why they matter to developers, and may be worth reviewing if you work with .NET, GitHub, Azure, or modern software development.";
        article.KeyTakeaways =
        "• Understand the main announcement\n" +
        "• Review any new APIs or features\n" +
        "• Consider whether this impacts your current projects";

        article.Category = DetermineCategory(article.Title + " " + article.Summary);
        article.IsAiProcessed = true;

        return Task.CompletedTask;
    }

    private string DetermineCategory(string? text)
    {
        text = text?.ToLower() ?? "";

        if (text.Contains("dotnet") || text.Contains(".net") || text.Contains("c#"))
            return ".NET";

        if (text.Contains("github") || text.Contains("copilot"))
            return "GitHub";

        if (text.Contains("azure") || text.Contains("cloud"))
            return "Cloud";

        if (text.Contains("ai") || text.Contains("openai"))
            return "AI";

        return "Technology";
    }
}