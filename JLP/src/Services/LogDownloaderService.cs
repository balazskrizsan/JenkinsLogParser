using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using JLP.Registries;
using JLP.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JLP.Services;

public class LogDownloaderService : ILogDownloaderService
{
    private readonly ILogger<ILogDownloaderService> logger;
    private readonly IApplicationArgumentRegistry applicationArgumentRegistry;

    static readonly HttpClient client = new HttpClient();

    public LogDownloaderService(
        ILogger<ILogDownloaderService> logger,
        IApplicationArgumentRegistry applicationArgumentRegistry
    )
    {
        this.logger = logger;
        this.applicationArgumentRegistry = applicationArgumentRegistry;
    }

    public async Task<List<LogResponse>> Download()
    {
        var lastLogId = applicationArgumentRegistry.LastLogId;
        var limit = applicationArgumentRegistry.Limit;

        var logResponses = new List<LogResponse>();

        for (var currentId = lastLogId; currentId > lastLogId - limit; currentId--)
        {
            var url = applicationArgumentRegistry.LogUrl.Replace("{id}", Convert.ToString(currentId));

            logger.LogInformation($"Downloading from: {url}", url);

            var response = await HttpGet(url, currentId);

            logger.LogInformation($"Downloading status: {response.HttpStatusCode}", response.HttpStatusCode);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                logResponses.Add(response);
            }
        }

        return logResponses;
    }

    public async Task<LogResponse> HttpGet(string url, int currentId)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        return new LogResponse(currentId, responseBody, response.StatusCode);
    }
}