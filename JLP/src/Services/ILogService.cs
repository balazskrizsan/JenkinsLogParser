using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Services;

public interface ILogService
{
    List<Log> getNewLogs();
}