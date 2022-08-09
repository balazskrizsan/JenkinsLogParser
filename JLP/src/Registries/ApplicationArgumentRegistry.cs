namespace JLP.Registries;

public class ApplicationArgumentRegistry : IApplicationArgumentRegistry
{
    public ApplicationArgumentRegistry(string lastLogId)
    {
        LastLogId = lastLogId;
    }

    public string LastLogId { get; }
}