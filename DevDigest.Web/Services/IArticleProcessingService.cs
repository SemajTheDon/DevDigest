namespace DevDigest.Web.Services;

public interface IArticleProcessingService
{
    Task ProcessArticleAsync(int articleId, CancellationToken cancellationToken = default);
}
