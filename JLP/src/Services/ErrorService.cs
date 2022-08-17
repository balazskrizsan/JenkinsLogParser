using System.Collections.Generic;
using JLP.Entities;
using JLP.Repositories;
using JLP.ValueObjects;

namespace JLP.Services;

public class ErrorService : IErrorService
{
    private readonly IErrorRepository errorRepository;

    public ErrorService(IErrorRepository errorRepository)
    {
        this.errorRepository = errorRepository;
    }

    public void SaveAll(List<LogError> logErrors)
    {
        logErrors.ForEach(logError => errorRepository.SaveAll(logError.Errors));
    }
}