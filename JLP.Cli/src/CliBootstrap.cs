using JLP.Registries;
using JLP.Services;
using Microsoft.Extensions.Logging;

namespace JLP.Cli;

public class CliBootstrap : ICliBootstrap
{
    private readonly IApplicationArgumentRegistry applicationArgumentRegistry;
    private readonly ILogErrorFinderService logErrorFinderService;
    private readonly IErrorService errorService;
    private readonly ILogDownloaderService logDownloaderService;
    private readonly ILogger<ICliBootstrap> logger;
    private readonly ILogService logService;

    public CliBootstrap(
        IApplicationArgumentRegistry applicationArgumentRegistry,
        ILogErrorFinderService logErrorFinderService,
        IErrorService errorService,
        ILogDownloaderService logDownloaderService,
        ILogger<ICliBootstrap> logger,
        ILogService logService
    )
    {
        this.applicationArgumentRegistry = applicationArgumentRegistry;
        this.logErrorFinderService = logErrorFinderService;
        this.errorService = errorService;
        this.logDownloaderService = logDownloaderService;
        this.logger = logger;
        this.logService = logService;
    }

    public async Task Start()
    {
        logger.LogInformation("== Downloading new logs");
        var logResponses = await logDownloaderService.Download();

        logger.LogInformation($"== Saving new logs; count {logResponses.Count}");
        logService.SaveAll(logResponses);
        
        logger.LogInformation("== Loading new logs");
        var logs = logService.SearchUnparsedLogs();
        
        logger.LogInformation($"== Searching for errors; count: {logs.Count}");
        var logErrors = logErrorFinderService.SearchErrors(logs);

        logger.LogInformation($"== Saving errors; count: {logErrors.Count}");
        errorService.SaveAll(logErrors);

        logger.LogInformation($"== Mark logs parsed; count: {logs.Count}");
        logService.MarkAllParsed(logs);
        
        logger.LogInformation("== Successful finish");
    }
}