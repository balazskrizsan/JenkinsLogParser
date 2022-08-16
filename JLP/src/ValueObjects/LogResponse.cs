using System.Net;

namespace JLP.ValueObjects;

public record LogResponse(
    int LogExternalId,
    string ResponseBody,
    HttpStatusCode HttpStatusCode
);