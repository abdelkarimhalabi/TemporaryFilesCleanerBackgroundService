namespace MyBackgroundService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ITempFileService _tempFileService;

    public Worker(ILogger<Worker> logger , ITempFileService tempFileService)
    {
        _logger = logger;
        _tempFileService = tempFileService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            _tempFileService.DeleteTempFiles();

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
