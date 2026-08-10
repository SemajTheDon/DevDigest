using DevDigest.Data.Data;
using DevDigest.Data.Models;
using DevDigest.Web.Services;
using Microsoft.AspNetCore.Mvc;
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
            .OrderByDescending(a => a.PublishedAt)
            .Take(10)
            .ToListAsync();

        foreach (var article in unprocessedArticles)
        {
            await _aiSummaryService.ProcessArticleAsync(article);
        }

        await _db.SaveChangesAsync();

        TotalArticles = await _db.Articles.CountAsync();
        DotNetCount = await _db.Articles.CountAsync(a => a.Category == ".NET");
        GitHubCount = await _db.Articles.CountAsync(a => a.Category == "GitHub");
        CloudCount = await _db.Articles.CountAsync(a => a.Category == "Cloud");
        AiCount = await _db.Articles.CountAsync(a => a.Category == "AI");

        // Start query
        var query = _db.Articles.AsQueryable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            query = query.Where(a =>
                a.Title.Contains(SearchTerm) ||
                a.Source.Contains(SearchTerm) ||
                a.Category!.Contains(SearchTerm));
        }

        // Category filter
        if (!string.IsNullOrWhiteSpace(Category))
        {
            query = query.Where(a => a.Category == Category);
        }

        // Sorting
        query = SortBy switch
        {
            "Oldest" => query.OrderBy(a => a.PublishedAt),
            "Source" => query.OrderBy(a => a.Source),
            _ => query.OrderByDescending(a => a.PublishedAt)
        };

        Articles = await query.ToListAsync();
    }

    [BindProperty(SupportsGet = true)]
    public string SearchTerm { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string Category { get; set; } = "";


    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "Newest";

    public int TotalArticles { get; set; }
    public int DotNetCount { get; set; }
    public int GitHubCount { get; set; }
    public int CloudCount { get; set; }
    public int AiCount { get; set; }
}