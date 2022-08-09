using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JLP.Entities;

namespace JLP.Services;

public class ErrorFinderService : IErrorFinderService
{
    private readonly Dictionary<int, Regex> errorPatterns = new()
    {
        { 1, new Regex(@"^[a-z][0-9]") },
        { 2, new Regex(@"^aa") }
    };

    public List<Error> SearchErrors(List<Log> logs)
    {
        var errors = new List<Error>();

        logs.ForEach(log =>
        {
            using (var reader = new StringReader(log.RawLog))
            {
                var lineNumber = 0;
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    lineNumber++;

                    var error = GetErrorFromLine(log.Id ?? 0, lineNumber, line);
                    if (error != null)
                    {
                        errors.Add(error);
                    }
                }
            }
        });

        return errors;
    }

    private Error GetErrorFromLine(int logId, int lineNumber, string line)
    {
        foreach (var (errorPatternId, errorPattern) in errorPatterns)
            if (errorPattern.IsMatch(line))
                return new Error(logId, line, lineNumber, errorPatternId);

        return null;
    }
}