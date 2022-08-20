using System;

namespace JLP.ValueObjects;

public record ParsedLine(string RawLine, int LineNumber, string Message, DateTime DateTime);