using JLP.Registries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace JLP.Cli;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var applicationArgumentRegistry = new ApplicationArgumentRegistry(args[0]);

        var host = AppStartup(applicationArgumentRegistry);

        await ActivatorUtilities.CreateInstance<CliBootstrap>(host.Services).Start();
    }

    private static IHost AppStartup(IApplicationArgumentRegistry applicationArgumentRegistry)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services
                    .ConfigureDependencies(applicationArgumentRegistry)
                    .SetupLogger();

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(
                        "Host=localhost;Database=stackjudge;Port=54330;Username=admin;Password=admin_pass;");
                });
            })
            .UseSerilog()
            .Build();
    }
}