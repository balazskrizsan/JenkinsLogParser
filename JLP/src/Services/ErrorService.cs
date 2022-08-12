using System.Collections.Generic;
using JLP.Entities;
using JLP.Repositories;

namespace JLP.Services;

public class ErrorService : IErrorService
{
    private readonly IErrorRepository errorRepository;

    public ErrorService(IErrorRepository errorRepository)
    {
        this.errorRepository = errorRepository;
    }

    public void SaveAll(List<Error> errors)
    {
        errorRepository.SaveAll(errors);
    }
}