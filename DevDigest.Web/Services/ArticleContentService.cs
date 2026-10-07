using HtmlAgilityPack;

namespace DevDigest.Web.Services;

public class ArticleContentService : IArticleContentService
{
    private readonly HttpClient _httpClient;
    public ArticleContentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetArticleContentAsync(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url, cancellationToken);

            var document = new HtmlDocument();
            document.LoadHtml(html);

            var nodesToRemove =
            document.DocumentNode.SelectNodes("//script|//style|//nav|//footer|//header");

            if (nodesToRemove != null)
            {
                foreach (var node in nodesToRemove)
                {
                    node.Remove();
                }
            }

            var text = document.DocumentNode.InnerText;

            text = string.Join(" ", text.Split(new[] { '\r', '\n', '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries));

            const int maxLength = 12000;
            if (text.Length > maxLength)
            {
                text = text[..maxLength];
            }
            return text;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article content from {url}: {ex.Message}");
            return string.Empty;
        }



    }
}
