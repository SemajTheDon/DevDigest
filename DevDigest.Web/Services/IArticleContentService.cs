namespace DevDigest.Web.Services;

public interface IArticleContentService
{
    Task<string> GetArticleContentAsync(string url, CancellationToken cancellationToken = default);
}
