using System.Collections.Generic;
using JLP.Entities;
using JLP.ValueObjects;

namespace JLP.Services;

public interface IErrorFinderService
{
    public List<LogError> SearchErrors(List<Log> logs);
}