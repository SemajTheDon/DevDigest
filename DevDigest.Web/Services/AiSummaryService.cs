using DevDigest.Data.Models;

namespace DevDigest.Web.Services;

public class AiSummaryService
{
    public Task ProcessArticleAsync(Article article)
    {
        article.AiSummary = $"Quick summary: {article.Title}";
        article.KeyTakeaways = "1. Important developer update\n2. Worth reviewing this week\n3. May apply to .NET/cloud learning";
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