using System.Net;

namespace JLP.ValueObjects;

public record LogResponse(string ResponseBody, HttpStatusCode HttpStatusCode);