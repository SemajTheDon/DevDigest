namespace DevDigest.Web.Services;

public interface IArticleProcessingQueue
{
    Task QueueArticleAsync(int articleId);

    Task<int> DequeueArticleAsync(CancellationToken cancellationToken);
}
