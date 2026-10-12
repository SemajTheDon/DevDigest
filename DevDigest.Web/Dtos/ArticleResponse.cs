namespace DevDigest.Web.Dtos;

public class ArticleResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Url { get; set; } = "";
    public string Source { get; set; } = "";
    public DateTime PublishedAt { get; set; }
    public string? Summary { get; set; }
    public string? Category { get; set; }
    public string? AiSummary { get; set; }
    public string? KeyTakeaways { get; set; }
}
