namespace JLP.Registries;

public class ApplicationArgumentRegistry : IApplicationArgumentRegistry
{
    public int LastLogId { get; }
    public int Limit { get; }
    public string LogUrl { get; }

    public ApplicationArgumentRegistry(int lastLogId, int limit, string logUrl)
    {
        LastLogId = lastLogId;
        Limit = limit;
        LogUrl = logUrl;
    }
}