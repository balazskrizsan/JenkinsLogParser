using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Repositories;

public interface IErrorRepository
{
    public void SaveAll(List<Error> errors);
}