using System.Collections.Generic;
using System.Threading.Tasks;
using JLP.ValueObjects;

namespace JLP.Services;

public interface ILogDownloaderService
{
    public Task<List<LogResponse>> Download();
}