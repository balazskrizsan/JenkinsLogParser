using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Services;

public interface ILineErrorFinderService
{
    Error SearchError(
        int logId,
        int lineNumber,
        ParsedLine parsedLine,
        Dictionary<int, ParsedLine> logLineCollector
    );
}