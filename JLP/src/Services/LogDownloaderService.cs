using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JLP.Registries;
using JLP.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JLP.Services;

public class LogDownloaderService : ILogDownloaderService
{
    private readonly ILogService logService;
    private readonly ILogger<ILogDownloaderService> logger;
    private readonly IApplicationArgumentRegistry applicationArgumentRegistry;

    static readonly HttpClient client = new HttpClient();

    public LogDownloaderService(
        ILogService logService,
        ILogger<ILogDownloaderService> logger,
        IApplicationArgumentRegistry applicationArgumentRegistry
    )
    {
        this.logService = logService;
        this.logger = logger;
        this.applicationArgumentRegistry = applicationArgumentRegistry;
    }

    public IEnumerable<int> CalculateNewExternalIds()
    {
        var lastLogId = applicationArgumentRegistry.LastLogId;
        var limit = applicationArgumentRegistry.Limit;

        var externalIdsToSelect = Enumerable.Range(lastLogId - limit + 1, limit).ToList();
        var externalIdsToSelectEnumerated = externalIdsToSelect.ToList();
        var externalIdsInDb = logService.GetExistingExternalIds(externalIdsToSelectEnumerated);

        return externalIdsToSelectEnumerated.Except(externalIdsInDb);
    }

    public async Task<List<LogResponse>> Download(IEnumerable<int> externalIds)
    {
        var logResponses = new List<LogResponse>();

        await Parallel.ForEachAsync(externalIds, async (externalId, token) =>
            {
                var url = applicationArgumentRegistry.LogUrl.Replace("{id}", Convert.ToString(externalId));
                var response = await CreateHttpGet(url, externalId);

                logger.LogInformation($"Downloading from: {url}", url);
                logger.LogInformation($"Downloading status: {response.HttpStatusCode}");

                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    logResponses.Add(response);
                }
            }
        );

        return logResponses;
    }

    private static async Task<LogResponse> CreateHttpGet(string url, int currentId)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        return new LogResponse(currentId, responseBody, response.StatusCode);
    }
}