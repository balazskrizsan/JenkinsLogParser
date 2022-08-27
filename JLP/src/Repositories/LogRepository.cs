using System.Collections.Generic;
using System.Linq;
using JLP.Entities;

namespace JLP.Repositories;

public class LogRepository : ILogRepository
{
    private readonly AppDbContext context;

    public LogRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<Log> SearchUnparsedLogs()
    {
        return context.Logs.Select(l => l).Where(l => !l.IsParsed).ToList();
    }

    public void SaveAll(IEnumerable<Log> logs)
    {
        context.Logs.AddRange(logs);
        context.SaveChanges();
    }

    public void MarkAllParsed(IEnumerable<int?> logIds)
    {
        context.Logs
            .Where(l => logIds.Contains(l.Id))
            .ToList()
            .ForEach(l => l.IsParsed = true);

        context.SaveChanges();
    }
}