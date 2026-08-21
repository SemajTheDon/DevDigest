namespace DevDigest.Web.Services;

public class DigestBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;

    public DigestBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue<bool>("DigestAutomation:Enabled");

        if (!enabled)
        {
            return;
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            await RunDigestAsync(stoppingToken);

            await Task.Delay(
                TimeSpan.FromHours(24),
                stoppingToken);
        }
    }

    private async Task RunDigestAsync(
        CancellationToken stoppingToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var automationService =
            scope.ServiceProvider
                .GetRequiredService<DigestAutomationService>();

        await automationService.RunDailyDigestAsync();
    }
}