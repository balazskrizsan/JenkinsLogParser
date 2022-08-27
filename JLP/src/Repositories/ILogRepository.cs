using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Repositories;

public interface ILogRepository
{
    List<Log> SearchUnparsedLogs();
    void SaveAll(List<LogResponse> logs);
    void MarkAllParsed(List<int?> logIds);
}