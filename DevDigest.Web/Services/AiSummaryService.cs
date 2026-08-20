#pragma warning disable OPENAI001

using DevDigest.Data.Models;
using OpenAI.Responses;

namespace DevDigest.Web.Services;

public class AiSummaryService
{
    private readonly ResponsesClient _client;
    private readonly string _model;

    public AiSummaryService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];

        _model = configuration["OpenAI:Model"]
            ?? "gpt-5.6-luna";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException(
                "OpenAI API key has not been configured.");
        }

        _client = new ResponsesClient(apiKey);
    }

    public async Task ProcessArticleAsync(Article article, string? articleContent = null)
    {
        var contentToAnalyze =
        !string.IsNullOrWhiteSpace(articleContent)
        ? articleContent
        : article.Summary;

        var prompt = $"""
        You are creating a daily digest for a software developer.

        Analyze the following developer article.

        Title:
        {article.Title}

        Source:
        {article.Source}

        Article description:
        {contentToAnalyze}

        Create:

        1. A concise summary between 2 and 4 sentences.
        2. Three useful key takeaways for a software developer.
        3. A category from one of these:
            .NET
            GitHub
            Cloud
            AI
            Database
            Web Development
            DevOps
            Security
            Technology

            Focus on what the developer could learn from the article.

        You MUST return the response using exactly this format:

        SUMMARY:
        [2-4 sentence summary]

        TAKEAWAYS:
        - [takeaway 1]
        - [takeaway 2]
        - [takeaway 3]

        CATEGORY:
        [one category from the provided list]

        Do not add headings, markdown formatting, or any other text.
        """;

        try
        {
            var options = new CreateResponseOptions
            {
                Model = _model,
            };

            options.InputItems.Add(
                ResponseItem.CreateUserMessageItem(prompt)
            );

            var response =
                await _client.CreateResponseAsync(options);

            string outputText =
                response.Value.GetOutputText();

            Console.WriteLine("AI RESPONSE:");

            var summaryStart =
                outputText.IndexOf("SUMMARY:", StringComparison.OrdinalIgnoreCase);

            var takeawaysStart =
                outputText.IndexOf("TAKEAWAYS:", StringComparison.OrdinalIgnoreCase);

            var categoryStart =
                outputText.IndexOf("CATEGORY:", StringComparison.OrdinalIgnoreCase);


            if (
                summaryStart >= 0 &&
                takeawaysStart >= 0 &&
                categoryStart >= 0)
            {
                article.AiSummary = outputText
                    .Substring(
                        summaryStart + "SUMMARY:".Length,
                        takeawaysStart -
                        (summaryStart + "SUMMARY:".Length))
                    .Trim();

                article.KeyTakeaways = outputText
                    .Substring(
                        takeawaysStart + "TAKEAWAYS:".Length,
                        categoryStart -
                        (takeawaysStart + "TAKEAWAYS:".Length))
                    .Trim();

                article.Category = outputText
                    .Substring(
                        categoryStart + "CATEGORY:".Length)
                    .Trim();

                article.IsAiProcessed = true;
            }
            else
            {
                Console.WriteLine(
                    "AI response format was unexpected.");

                article.IsAiProcessed = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Error processing article with AI: {ex.Message}");

            article.IsAiProcessed = false;
        }

    }

    private static string DetermineCategory(string? text)
    {
        text = text?.ToLower() ?? string.Empty;

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