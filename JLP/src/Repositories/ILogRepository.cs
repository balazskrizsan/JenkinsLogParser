using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Repositories;

public interface ILogRepository
{
    public List<Log> Search();
}