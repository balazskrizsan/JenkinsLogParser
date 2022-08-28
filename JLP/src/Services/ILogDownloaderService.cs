using System.Collections.Generic;
using System.Threading.Tasks;
using JLP.ValueObjects;

namespace JLP.Services;

public interface ILogDownloaderService
{
    public IEnumerable<int> CalculateNewExternalIds();
    Task<List<LogResponse>> Download(IEnumerable<int> externalIds);
}