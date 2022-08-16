using System;
using System.Collections.Generic;
using System.Linq;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Repositories;

public class LogRepository : ILogRepository
{
    private readonly AppDbContext context;

    public LogRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<Log> Search()
    {
        return context.Logs.Select(l => l).ToList();
    }

    public void SaveAll(List<LogResponse> logs)
    {
        var mappedLogs = logs.Select(log => new Log
        {
            LogExternalId = log.LogExternalId,
            RawLog = log.ResponseBody,
            CreatedAt = DateTime.UtcNow
        });

        context.Logs.AddRange(mappedLogs);
        context.SaveChanges();
    }
}