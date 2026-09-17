using DevDigest.Data.Data;
using DevDigest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Web.Pages;

public class ArticlesModel : PageModel
{
    private readonly AppDbContext _db;

    public ArticlesModel(AppDbContext db)
    {
        _db = db;
    }

    public List<Article> Articles { get; set; } = [];

    public async Task OnGetAsync()
    {
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