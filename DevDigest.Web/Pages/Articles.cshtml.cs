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

    public ArticlesModel(
        AppDbContext db,
        RssFeedService rss)
    {
        _db = db;
        _rss = rss;
    }

    public List<Article> Articles { get; set; } = [];

    public async Task OnGetAsync()
    {
        await _rss.ImportFeedsAsync();

        Articles = await _db.Articles
            .OrderByDescending(x => x.PublishedAt)
            .ToListAsync();
    }
}