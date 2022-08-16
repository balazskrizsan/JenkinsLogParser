using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Services;

public interface ILogService
{
    List<Log> GetNewLogs();
    void SaveAll(List<LogResponse> logs);
}