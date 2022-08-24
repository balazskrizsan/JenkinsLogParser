using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Services;

public interface ILogErrorFinderService
{
    public List<LogError> SearchErrors(List<Log> logs);
}