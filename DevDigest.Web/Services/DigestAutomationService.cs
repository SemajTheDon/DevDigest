namespace DevDigest.Web.Services;

public class DigestAutomationService
{
    private readonly RssFeedService _rssFeedService;
    private readonly DailyDigestService _dailyDigestService;

    public DigestAutomationService(RssFeedService rssFeedService, DailyDigestService dailyDigestService)
    {
        _rssFeedService = rssFeedService;
        _dailyDigestService = dailyDigestService;
    }

    public async Task RunDailyDigestAsync()
    {
        Console.WriteLine("Starting daily digest automation...");
        try
        {
            Console.WriteLine("Importing RSS feeds...");
            await _rssFeedService.ImportFeedsAsync();
            Console.WriteLine("RSS import completed.");
            Console.WriteLine("Sending daily digest email...");
            await _dailyDigestService.SendDailyDigestEmailAsync();
            Console.WriteLine("Daily digest email sent successfully.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during daily digest automation: {ex.Message}");
        }
    }
}