using MyBackgroundService;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

var workerName = "TempFileCleanerByAbdelkarim";
LogManager.Setup().LoadConfigurationFromAppSettings();
var logger = LogManager.GetCurrentClassLogger();

try
{
    logger.Info("Starting application...");

    // Create the builder
    var builder = Host.CreateApplicationBuilder(args);
    var nlogSection = builder.Configuration.GetSection("NLog");
    var conf = builder.Configuration;

    // Configure NLog as the logging provider
    builder.Logging.ClearProviders(); // Remove default providers
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace); // Set the minimum level
    builder.Logging.AddNLog(conf); // Add NLog from appsettings.json

    // Set WorkerName as an NLog global property AFTER the configuration is loaded
    LogManager.Configuration.Variables["WorkerName"] = workerName;

    // Register services
    builder.Services.AddSingleton<ITempFileService, TempFileService>();
    builder.Services.AddHostedService<Worker>();

    // Build and run the application
    var host = builder.Build();
    host.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Application terminated unexpectedly at {time}. Exception: {exceptionMessage}", DateTimeOffset.Now, ex.Message);
    throw;
}
finally
{
    LogManager.Shutdown();
}