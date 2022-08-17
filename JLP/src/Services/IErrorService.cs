using System.Collections.Generic;
using JLP.ValueObjects;

namespace JLP.Services;

public interface IErrorService
{
    void SaveAll(List<LogError> logErrors);
}