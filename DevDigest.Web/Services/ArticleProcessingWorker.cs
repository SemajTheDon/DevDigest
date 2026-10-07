using DevDigest.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Web.Services;

public class ArticleProcessingWorker : BackgroundService
{
    private readonly IArticleProcessingQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ArticleProcessingWorker> _logger;

    public ArticleProcessingWorker(
        IArticleProcessingQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<ArticleProcessingWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        // Saved articles are the durable record of pending work after a restart.
        try
        {
            await using var recoveryScope = _scopeFactory.CreateAsyncScope();
            var db = recoveryScope.ServiceProvider.GetRequiredService<AppDbContext>();
            var pendingIds = await db.Articles.AsNoTracking()
                .Where(article => !article.IsAiProcessed)
                .OrderBy(article => article.Id)
                .Select(article => article.Id)
                .ToListAsync(stoppingToken);

            foreach (var articleId in pendingIds)
            {
                stoppingToken.ThrowIfCancellationRequested();
                await _queue.QueueArticleAsync(articleId);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            int articleId;
            try
            {
                articleId = await _queue.DequeueArticleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var processor = scope.ServiceProvider
                    .GetRequiredService<IArticleProcessingService>();

                await processor.ProcessArticleAsync(articleId, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing article {ArticleId}", articleId);
            }
        }
    }
}
