using System;
using System.Collections.Generic;
using System.Linq;
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

    public List<Log> SearchUnparsedLogs()
    {
        return logRepository.SearchUnparsedLogs();
    }

    public void SaveAll(List<LogResponse> logs)
    {
        var mappedLogs = logs.Select(log => new Log
        {
            IsParsed = false,
            LogExternalId = log.LogExternalId,
            RawLog = log.ResponseBody,
            CreatedAt = DateTime.UtcNow
        });

        logRepository.SaveAll(mappedLogs);
    }

    public void MarkAllParsed(List<Log> logs)
    {
        logRepository.MarkAllParsed(logs.Select(l => l.Id).ToList());
    }
}