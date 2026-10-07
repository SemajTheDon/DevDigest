using DevDigest.Data.Data;

namespace DevDigest.Web.Services;

public class ArticleProcessingService : IArticleProcessingService
{
    private readonly AppDbContext _db;
    private readonly IArticleContentService _articleContentService;
    private readonly IAiSummaryService _aiSummaryService;

    public ArticleProcessingService(
        AppDbContext db,
        IArticleContentService articleContentService,
        IAiSummaryService aiSummaryService)
    {
        _db = db;
        _articleContentService = articleContentService;
        _aiSummaryService = aiSummaryService;
    }

    public async Task ProcessArticleAsync(int articleId, CancellationToken cancellationToken = default)
    {
        // 1. Find article from database
        var article = await _db.Articles.FindAsync(new object[] { articleId }, cancellationToken);

        // 2. Handle article not found
        if (article == null || article.IsAiProcessed)
        {
            // Handle the case where the article is not found
            return;
        }

        // 3. Get full article content
        var fullContent = await _articleContentService.GetArticleContentAsync(article.Url, cancellationToken);

        // 4. Process with AI
        await _aiSummaryService.ProcessArticleAsync(article, fullContent, cancellationToken);

        // 5. Save AI changes
        await _db.SaveChangesAsync(cancellationToken);
    }
}
