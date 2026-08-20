using HtmlAgilityPack;

namespace DevDigest.Web.Services;

public class ArticleContentService
{
    private readonly HttpClient _httpClient;
    public ArticleContentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetArticleContentAsync(string url)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(url);

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
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching article content from {url}: {ex.Message}");
            return string.Empty;
        }



    }
}
