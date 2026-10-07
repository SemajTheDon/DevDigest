using DevDigest.Data.Models;

namespace DevDigest.Web.Services;

public interface IAiSummaryService
{
    Task ProcessArticleAsync(Article article, string? articleContent = null, CancellationToken cancellationToken = default);
}
