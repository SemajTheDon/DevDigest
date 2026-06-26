using DevDigest.Data.Data;
using DevDigest.Data.Models;
using DevDigest.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Web.Pages;

public class ArticlesModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly RssFeedService _rss;
    private readonly AiSummaryService _aiSummaryService;

    public ArticlesModel(
        AppDbContext db,
        RssFeedService rss,
        AiSummaryService aiSummaryService)
    {
        _db = db;
        _rss = rss;
        _aiSummaryService = aiSummaryService;
    }

    public List<Article> Articles { get; set; } = [];

    public async Task OnGetAsync()
    {
        await _rss.ImportFeedsAsync();

        var unprocessedArticles = await _db.Articles
            .Where(a => !a.IsAiProcessed)
            .Take(10)
            .ToListAsync();

        foreach (var article in unprocessedArticles)
        {
            await _aiSummaryService.ProcessArticleAsync(article);
        }

        await _db.SaveChangesAsync();

        Articles = await _db.Articles
            .OrderByDescending(x => x.PublishedAt)
            .ToListAsync();
    }
}