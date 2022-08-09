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

    public List<Log> Search()
    {
        return context.Logs.Select(l => l).ToList();
    }
}