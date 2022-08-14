namespace JLP.Registries;

public interface IApplicationArgumentRegistry
{
    int LastLogId { get; }
    int Limit { get; }
    string LogUrl { get; }
}