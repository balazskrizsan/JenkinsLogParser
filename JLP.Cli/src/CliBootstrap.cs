using JLP.Registries;
using JLP.Services;
using Microsoft.Extensions.Logging;

namespace JLP.Cli;

public class CliBootstrap : ICliBootstrap
{
    private readonly IApplicationArgumentRegistry applicationArgumentRegistry;
    private readonly IErrorFinderService errorFinderService;
    private readonly IErrorService errorService;
    private readonly ILogger<ICliBootstrap> logger;
    private readonly ILogService logService;

    public CliBootstrap(
        ILogService logService,
        IErrorService errorService,
        IErrorFinderService errorFinderService,
        IApplicationArgumentRegistry applicationArgumentRegistry,
        ILogger<CliBootstrap> logger
    )
    {
        this.logService = logService;
        this.errorService = errorService;
        this.errorFinderService = errorFinderService;
        this.applicationArgumentRegistry = applicationArgumentRegistry;
        this.logger = logger;
    }

    public void Start()
    {
        logger.LogInformation("====== Loading new logs");

        var logs = logService.getNewLogs();

        logger.LogInformation("====== Searching for errors");
        
        var errors = errorFinderService.SearchErrors(logs);
        
        logger.LogInformation("====== Saving errors");
        
        errorService.SaveAll(errors);

        logger.LogInformation("====== Successful finish");
    }
}