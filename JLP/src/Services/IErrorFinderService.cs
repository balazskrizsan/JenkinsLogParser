using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Services;

public interface IErrorFinderService
{
    public List<Error> SearchErrors(List<Log> logs);
}