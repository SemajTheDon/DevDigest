using CodeHollow.FeedReader;
using DevDigest.Data.Data;
using DevDigest.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DevDigest.Web.Services;

public class RssFeedService
{
    private readonly AppDbContext _db;

    public RssFeedService(AppDbContext db)
    {
        _db = db;
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
                    Category = "Technology",
                    PublishedAt = item.PublishingDate ?? DateTime.UtcNow
                };

                _db.Articles.Add(article);
            }
        }

        await _db.SaveChangesAsync();
    }
}
