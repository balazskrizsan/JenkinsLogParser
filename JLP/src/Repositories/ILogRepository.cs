using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Repositories;

public interface ILogRepository
{
    public List<Log> SearchUnparsedLogs();
    public void SaveAll(List<LogResponse> logs);
}