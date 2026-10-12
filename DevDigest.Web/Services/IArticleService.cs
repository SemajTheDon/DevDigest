using DevDigest.Web.Dtos;

namespace DevDigest.Web.Services;

public interface IArticleService
{
    Task<ArticleResponse?> GetArticleAsync(int articleId);
}
