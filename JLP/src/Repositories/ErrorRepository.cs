using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Repositories;

public class ErrorRepository : IErrorRepository
{
    private readonly AppDbContext context;

    public ErrorRepository(AppDbContext context)
    {
        this.context = context;
    }

    public void SaveAll(List<Error> errors)
    {
        context.Errors.AddRange(errors);
        context.SaveChanges();
    }
}