using System.Threading.Tasks;

namespace JLP.Services;

public interface ILogDownloaderService
{
    public Task Download();
}