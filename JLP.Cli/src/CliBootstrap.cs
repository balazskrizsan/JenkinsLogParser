using JLP.Registries;
using JLP.Services;
using Microsoft.Extensions.Logging;

namespace JLP.Cli;

public class CliBootstrap : ICliBootstrap
{
    private readonly IApplicationArgumentRegistry applicationArgumentRegistry;
    private readonly IErrorFinderService errorFinderService;
    private readonly ILogger<ICliBootstrap> logger;
    private readonly ILogService logService;

    public CliBootstrap(
        ILogService logService,
        IErrorFinderService errorFinderService,
        IApplicationArgumentRegistry applicationArgumentRegistry,
        ILogger<CliBootstrap> logger
    )
    {
        this.logService = logService;
        this.errorFinderService = errorFinderService;
        this.applicationArgumentRegistry = applicationArgumentRegistry;
        this.logger = logger;
    }

    public void Start()
    {
        var logs = logService.getNewLogs();

        var errors = errorFinderService.SearchErrors(logs);


        logger.LogInformation("====== Successful finish");
    }
}