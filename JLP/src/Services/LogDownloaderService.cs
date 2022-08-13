using System.Net.Http;
using System.Threading.Tasks;
using JLP.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JLP.Services;

public class LogDownloaderService : ILogDownloaderService
{
    private readonly ILogger<ILogDownloaderService> logger;

    static readonly HttpClient client = new HttpClient();

    public LogDownloaderService(ILogger<ILogDownloaderService> logger)
    {
        this.logger = logger;
    }

    public async Task Download()
    {
        var url = "";
        logger.LogInformation($"Downloading from: {url}", url);

        var response = await HttpGet(url);

        logger.LogInformation($"Downloading status: {response.HttpStatusCode}", response.HttpStatusCode);
    }

    public async Task<LogResponse> HttpGet(string url)
    {
        HttpResponseMessage response = await client.GetAsync(url);
        var responseBody = await response.Content.ReadAsStringAsync();

        return new LogResponse(responseBody, response.StatusCode);
    }
}