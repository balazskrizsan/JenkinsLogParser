using JLP.Registries;
using JLP.Services;
using Microsoft.Extensions.Logging;

namespace JLP.Cli;

public class CliBootstrap : ICliBootstrap
{
    private readonly IApplicationArgumentRegistry applicationArgumentRegistry;
    private readonly IErrorFinderService errorFinderService;
    private readonly IErrorService errorService;
    private readonly ILogDownloaderService logDownloaderService;
    private readonly ILogger<ICliBootstrap> logger;
    private readonly ILogService logService;

    public CliBootstrap(
        IApplicationArgumentRegistry applicationArgumentRegistry,
        IErrorFinderService errorFinderService,
        IErrorService errorService,
        ILogDownloaderService logDownloaderService,
        ILogger<ICliBootstrap> logger,
        ILogService logService
    )
    {
        this.applicationArgumentRegistry = applicationArgumentRegistry;
        this.errorFinderService = errorFinderService;
        this.errorService = errorService;
        this.logDownloaderService = logDownloaderService;
        this.logger = logger;
        this.logService = logService;
    }

    public async Task Start()
    {
        logger.LogInformation("== Downloading new logs");

        var logResponses = await logDownloaderService.Download();

        logService.SaveAll(logResponses);
        
        logger.LogInformation("== Loading new logs");
        
        var logs = logService.SearchUnparsedLogs();
        
        logger.LogInformation("== Searching for errors");
        
        var logErrors = errorFinderService.SearchErrors(logs);

        logger.LogInformation("== Saving errors");
        
        errorService.SaveAll(logErrors);

        logger.LogInformation("== Successful finish");
    }
}