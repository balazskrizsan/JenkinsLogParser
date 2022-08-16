using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Repositories;

public interface ILogRepository
{
    public List<Log> Search();
    public void SaveAll(List<LogResponse> logs);
}