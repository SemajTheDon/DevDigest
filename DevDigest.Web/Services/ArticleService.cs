using DevDigest.Data.Data;
using DevDigest.Web.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Web.Services;

public class ArticleService : IArticleService
{
    private readonly AppDbContext _db;

    public ArticleService(AppDbContext db) => _db = db;

    public Task<ArticleResponse?> GetArticleAsync(int articleId) =>
        _db.Articles
            .AsNoTracking()
            .Where(article => article.Id == articleId)
            .Select(article => new ArticleResponse
            {
                Id = article.Id,
                Title = article.Title,
                Url = article.Url,
                Source = article.Source,
                PublishedAt = article.PublishedAt,
                Summary = article.Summary,
                Category = article.Category,
                AiSummary = article.AiSummary,
                KeyTakeaways = article.KeyTakeaways
            })
            .FirstOrDefaultAsync();
}
