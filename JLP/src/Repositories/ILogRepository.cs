using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Repositories;

public interface ILogRepository
{
    List<Log> SearchUnparsedLogs();
    void SaveAll(IEnumerable<Log> logs);
    void MarkAllParsed(IEnumerable<int?> logIds);
}