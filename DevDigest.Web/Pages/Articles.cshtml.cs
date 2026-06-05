using DevDigest.Data.Data;
using DevDigest.Data.Models;
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
        if (!await _db.Articles.AnyAsync())
        {
            _db.Articles.Add(new Article
            {
                Title = "Welcome to DevDigest",
                Url = "https://example.com",
                Source = ".NET Blog",
                Category = ".NET",
                Summary = "First article",
                PublishedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        Articles = await _db.Articles
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();
    }
}