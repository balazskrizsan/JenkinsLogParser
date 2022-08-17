using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Services;

public interface ILogService
{
    List<Log> SearchUnparsedLogs();
    void SaveAll(List<LogResponse> logs);
}