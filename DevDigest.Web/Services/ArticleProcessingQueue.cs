using System.Threading.Channels;

namespace DevDigest.Web.Services;

public class ArticleProcessingQueue : IArticleProcessingQueue
{
    private readonly Channel<int> _queue;

    public ArticleProcessingQueue() => _queue = Channel.CreateUnbounded<int>();

    public async Task QueueArticleAsync(int articleId) =>
        await _queue.Writer.WriteAsync(articleId);

    public async Task<int> DequeueArticleAsync(
        CancellationToken cancellationToken) =>
        await _queue.Reader.ReadAsync(cancellationToken);
}
