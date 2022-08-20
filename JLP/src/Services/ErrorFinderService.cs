using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JLP.Entities;
using JLP.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JLP.Services;

public class ErrorFinderService : IErrorFinderService
{
    private ILogger<IErrorFinderService> logger;

    public ErrorFinderService(ILogger<IErrorFinderService> logger)
    {
        this.logger = logger;
    }

    // Time regex: \[\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{1,4}Z\]  
    private readonly Dictionary<int, Regex> errorPatterns = new()
    {
        { 1, new Regex(@"^[ ]{2}\d{1,3}\)[ ].*") },
    };

    public List<LogError> SearchErrors(List<Log> logs)
    {
        var logErrors = new List<LogError>();

        logs.ForEach(log =>
        {
            var errors = new List<Error>();
            Dictionary<int, ParsedLine> logLineCollector = new();

            using (var reader = new StringReader(log.RawLog))
            {
                logger.LogInformation($"==== Parse lines in xidf#{log.LogExternalId}");

                var lineNumber = 0;

                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine(), lineNumber++)
                {
                    var parsedLine = ParseLine(line, lineNumber);
                    logLineCollector.Add(parsedLine.LineNumber, parsedLine);
                }
            }

            foreach (var (lineNumber, parsedLine) in logLineCollector)
            {
                var error = GetErrorFromLine(log.Id ?? 0, lineNumber, parsedLine, logLineCollector);
                if (error != null)
                {
                    errors.Add(error);
                }
            }

            logger.LogInformation($"     Found errors: {errors.Count}");
            logger.LogInformation($"     First message: {logLineCollector[0].RawLine}");
            logger.LogInformation($"     Last message: {logLineCollector.Values.Last().RawLine}");

            logErrors.Add(new LogError(errors.Count > 0, errors));
        });

        return logErrors;
    }

    private Error GetErrorFromLine(
        int logId,
        int lineNumber,
        ParsedLine parsedLine,
        Dictionary<int, ParsedLine> logLineCollector
    )
    {
        foreach (var (errorPatternId, errorPattern) in errorPatterns)
        {
            if (errorPattern.IsMatch(parsedLine.Message))
            {
                var context = String.Join(
                    Environment.NewLine,
                    logLineCollector
                        .Select(d => d.Value)
                        .Skip(parsedLine.LineNumber - 5)
                        .Take(10)
                        .Select(l => $"#{l.LineNumber}: [{l.DateTime.ToUniversalTime()}] {l.Message}")
                );

                return new Error(
                    logId,
                    lineNumber,
                    errorPatternId,
                    parsedLine.Message,
                    parsedLine.DateTime,
                    parsedLine.RawLine,
                    context
                );
            }
        }

        return null;
    }

    private ParsedLine ParseLine(string line, int lineNumber)
    {
        string lineParser = @"\[(?<Date>\d{4}-\d{2}-\d{2})T(?<Time>\d{2}:\d{2}:\d{2}\.\d{1,4})Z\][ ](?<Message>.*)";

        var results = new Regex(lineParser).Matches(line);

        var dateTime = DateTime.UtcNow;
        var message = "";

        foreach (Match match in results)
        {
            message = match.Groups["Message"].Value;
            dateTime = DateTime.Parse($"{match.Groups["Date"].Value} {match.Groups["Time"].Value}")
                .ToUniversalTime();
        }

        return new ParsedLine(line, lineNumber, message, dateTime);
    }
}