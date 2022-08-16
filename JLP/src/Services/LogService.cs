using System.Collections.Generic;
using JLP.Entities;
using JLP.Repositories;
using JLP.ValueObjects;

namespace JLP.Services;

public class LogService : ILogService
{
    private readonly ILogRepository logRepository;

    public LogService(ILogRepository logRepository)
    {
        this.logRepository = logRepository;
    }

    public List<Log> GetNewLogs()
    {
        return logRepository.Search();
    }

    public void SaveAll(List<LogResponse> logs)
    {
        logRepository.SaveAll(logs);
    }
}