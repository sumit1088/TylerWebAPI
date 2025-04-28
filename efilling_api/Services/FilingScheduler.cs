using Microsoft.Extensions.Hosting;
using efilling_api.Services;

namespace efilling_api.Services
{
    //public class FilingScheduler : BackgroundService
    //{
    //    private readonly IServiceScopeFactory _scopeFactory;
    //    private readonly ILogger<FilingScheduler> _logger;

    //    public FilingScheduler(IServiceScopeFactory scopeFactory, ILogger<FilingScheduler> logger)
    //    {
    //        _scopeFactory = scopeFactory;
    //        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    //    }

    //    public void Process()
    //    {
    //        _logger.LogInformation("Processing started at {Time}", DateTime.UtcNow);
    //    }

    //    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    //{
    //    //    _logger.LogInformation("🚀 Background service started at {Time}", DateTime.UtcNow);

    //    //    while (!stoppingToken.IsCancellationRequested)
    //    //    {
    //    //        _logger.LogInformation("🔄 Starting FetchAndStoreFilingsAsync at {Time}", DateTime.UtcNow);

    //    //        try
    //    //        {
    //    //            using (var scope = _scopeFactory.CreateScope())
    //    //            {
    //    //                var filingService = scope.ServiceProvider.GetRequiredService<FilingService>();
    //    //                await filingService.FetchAndStoreFilingsAsync();
    //    //                _logger.LogInformation("✅ FetchAndStoreFilingsAsync completed at {Time}", DateTime.UtcNow);
    //    //            }
    //    //        }
    //    //        catch (Exception ex)
    //    //        {
    //    //            _logger.LogError(ex, "❌ An error occurred while fetching filings.");
    //    //        }

    //    //        _logger.LogInformation("⏳ Waiting 5 minutes before next execution...");

    //    //        try
    //    //        {
    //    //            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
    //    //        }
    //    //        catch (TaskCanceledException)
    //    //        {
    //    //            _logger.LogWarning("🛑 Task was canceled. Stopping service...");
    //    //            break;
    //    //        }
    //    //    }

    //    //    _logger.LogInformation("🛑 Background service stopped at {Time}", DateTime.UtcNow);
    //    //}


    //}
}

