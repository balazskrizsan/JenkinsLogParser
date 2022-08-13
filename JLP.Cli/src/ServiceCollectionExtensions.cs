using JLP.Registries;
using JLP.Repositories;
using JLP.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace JLP.Cli;

public static class ConfigureServicesHelperExtensions
{
    public static IServiceCollection ConfigureDependencies(
        this IServiceCollection serviceCollection,
        IApplicationArgumentRegistry applicationArgumentRegistry
    )
    {
        return serviceCollection
                .AddLogging()
                .AddSingleton(_ => applicationArgumentRegistry)
                .AddSingleton<IErrorFinderService, ErrorFinderService>()
                .AddSingleton<ILogService, LogService>()
                .AddSingleton<ILogRepository, LogRepository>()
                .AddSingleton<IErrorService, ErrorService>()
                .AddSingleton<IErrorRepository, ErrorRepository>()
                .AddSingleton<ILogDownloaderService, LogDownloaderService>()
            ;
    }

    public static IServiceCollection SetupLogger(this IServiceCollection serviceCollection)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
            .AddJsonFile("appsettings.json", false)
            .Build();

        serviceCollection.AddSingleton(configuration);

        Log.Logger = new LoggerConfiguration() // initiate the logger configuration
            .ReadFrom.Configuration(configuration) // connect serilog to our configuration folder
            .Enrich.FromLogContext() //Adds more information to our logs from built in Serilog 
            .WriteTo.Console() // decide where the logs are going to be shown
            .CreateLogger();

        return serviceCollection;
    }
}