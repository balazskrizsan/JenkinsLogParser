using System.Collections.Generic;
using JLP.Entities;

namespace JLP.Services;

public interface IErrorService
{
    void SaveAll(List<Error> errors);
}