namespace DevDigest.Data.Models;

public class Article
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Url { get; set; } = "";

    public string Source { get; set; } = "";

    public DateTime PublishedAt { get; set; }

    public string? Summary { get; set; }

    public string? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? AiSummary { get; set; }

    public string? KeyTakeaways { get; set; }

    public bool IsAiProcessed { get; set; } = false;
}