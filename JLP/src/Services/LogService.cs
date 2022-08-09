using System.Collections.Generic;
using JLP.Entities;
using JLP.Repositories;

namespace JLP.Services;

public class LogService : ILogService
{
    private readonly ILogRepository logRepository;

    public LogService(ILogRepository logRepository)
    {
        this.logRepository = logRepository;
    }

    public List<Log> getNewLogs()
    {
        return logRepository.Search();
    }
}