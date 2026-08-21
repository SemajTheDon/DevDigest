using DevDigest.Data.Data;
using DevDigest.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DevDigest.Web.Services;

public class DailyDigestService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public DailyDigestService(AppDbContext db, IConfiguration configuration, HttpClient httpClient)
    {
        _db = db;
        _configuration = configuration;
        _httpClient = httpClient;
    }


    public async Task<List<Article>> GetDigestArticlesAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-1);

        return await _db.Articles
            .Where(a => a.IsAiProcessed && a.PublishedAt >= cutoffDate)
            .OrderByDescending(a => a.PublishedAt)
            .Take(5)
            .ToListAsync();
    }

    private string BuildEmailHtml(List<Article> articles)
    {
        var html = """
        <html>
        <body style="margin:0; padding:24px; background-color:#f4f4f5; font-family:Arial, Helvetica, sans-serif; color:#18181b;">

        <h1 style="margin:0 0 16px; font-size:22px;">Daily Developer Digest</h1>

        <p style="margin:0 0 24px; font-size:14px; line-height:1.5;">
            Here are today's developer articles.
        </p>
        """;

        foreach (var article in articles)
        {
            html += $"""
            <div style="margin:0 0 24px; padding:20px; background-color:#ffffff; border:1px solid #e4e4e7; border-radius:8px;">

                <h2 style="margin:0 0 8px; font-size:18px;">{article.Title}</h2>

                <p style="margin:0 0 12px; font-size:13px; color:#71717a;">
                    <strong>{article.Source}</strong>
                    • {article.Category}
                </p>

                <p style="margin:0 0 16px; font-size:14px; line-height:1.5;">
                    {article.AiSummary}
                </p>

                {BuildKeyTakeawaysHtml(article.KeyTakeaways)}

                <p style="margin:16px 0 0; font-size:14px;">
                    <a href="{article.Url}" style="color:#2563eb;">
                        Read Full Article
                    </a>
                </p>

            </div>
            """;
        }

        html += """
        </body>
        </html>
        """;

        return html;
    }

    private static string BuildKeyTakeawaysHtml(string? keyTakeaways)
    {
        if (string.IsNullOrWhiteSpace(keyTakeaways))
        {
            return "";
        }

        var takeaways = keyTakeaways
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().TrimStart('-', '•').Trim())
            .Where(x => x.Length > 0);

        var items = string.Join("", takeaways.Select(t => $"""<li style="margin:0 0 6px; font-size:14px; line-height:1.5;">{t}</li>"""));

        return $"""
        <h3 style="margin:0 0 8px; font-size:15px;">Key Takeaways</h3>
        <ul style="margin:0; padding-left:20px;">
            {items}
        </ul>
        """;
    }

    public async Task SendDailyDigestEmailAsync()
    {
        var articles = await GetDigestArticlesAsync();

        if (articles.Count == 0)
        {
            Console.WriteLine("No articles available for today's digest.");
            return;
        }

        var html = BuildEmailHtml(articles);

        var apiKey = _configuration["Resend:ApiKey"];
        var toEmail = _configuration["Digest:ToEmail"];
        var fromEmail = _configuration["Digest:FromEmail"];

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(toEmail) ||
            string.IsNullOrWhiteSpace(fromEmail))
        {
            Console.WriteLine("Resend email configuration is missing.");
            return;
        }

        var request = new
        {
            from = fromEmail,
            to = new[] { toEmail },
            subject = $"Daily Developer Digest - {DateTime.Now:MMMM d, yyyy}",
            html = html
        };

        using var message =
            new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.resend.com/emails");

        message.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                apiKey);

        message.Content =
            JsonContent.Create(request);

        try
        {
            var response =
                await _httpClient.SendAsync(message);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Daily digest email sent successfully.");
                Console.WriteLine(responseBody);
            }
            else
            {
                Console.WriteLine(
                    $"Failed to send digest: {(int)response.StatusCode}");

                Console.WriteLine(responseBody);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Error sending daily digest: {ex.Message}");
        }
    }
}