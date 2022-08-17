using System.Collections.Generic;
using JLP.Entities;

namespace JLP.ValueObjects;

public record LogError(bool IsFailed, List<Error> Errors);