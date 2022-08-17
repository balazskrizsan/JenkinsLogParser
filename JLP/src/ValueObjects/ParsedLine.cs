using System;

namespace JLP.ValueObjects;

public record ParsedLine(string RawLine, string Message, DateTime DateTime);