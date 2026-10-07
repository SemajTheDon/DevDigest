using CodeHollow.FeedReader;
using DevDigest.Data.Data;
using DevDigest.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Web.Services;

public class RssFeedService
{
    private readonly AppDbContext _db;
    private readonly IArticleProcessingQueue _articleProcessingQueue;

    public RssFeedService(AppDbContext db, IArticleProcessingQueue articleProcessingQueue)
    {
        _db = db;
        _articleProcessingQueue = articleProcessingQueue;
    }

    public async Task ImportFeedsAsync()
    {
        var feeds = new Dictionary<string, string>
        {
            { ".NET Blog", "https://devblogs.microsoft.com/dotnet/feed/" },
            { "GitHub Blog", "https://github.blog/feed/" }
        };

        foreach (var feed in feeds)
        {
            var rss = await FeedReader.ReadAsync(feed.Value);

            foreach (var item in rss.Items.Take(10))
            {
                bool exists = await _db.Articles
                    .AnyAsync(x => x.Url == item.Link);

                if (exists)
                    continue;

                var article = new Article
                {
                    Title = item.Title ?? "Unknown",
                    Url = item.Link ?? "",
                    Source = feed.Key,
                    Summary = item.Description,
                    Category = string.Empty,
                    PublishedAt = item.PublishingDate ?? DateTime.UtcNow
                };

                _db.Articles.Add(article);

                // Persist article before external processing
                await _db.SaveChangesAsync();

                try
                {
                    await _articleProcessingQueue.QueueArticleAsync(article.Id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error queuing article {article.Id}: {ex.Message}");
                }
            }
        }
    }
}
