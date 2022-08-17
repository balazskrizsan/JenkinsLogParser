using System;
using System.Collections.Generic;
using System.IO;
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

            using (var reader = new StringReader(log.RawLog))
            {
                logger.LogInformation($"==== Find errors in xidf#{log.LogExternalId}");

                var lineNumber = 0;
                string lastMessage = String.Empty;
                string firstMessage = String.Empty;

                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    var parsedLine = ParseLine(line);

                    if (lineNumber == 0)
                    {
                        firstMessage = parsedLine.Message;
                    }

                    lineNumber++;

                    var error = GetErrorFromLine(log.Id ?? 0, lineNumber, parsedLine);
                    if (error != null)
                    {
                        errors.Add(error);
                    }

                    lastMessage = line;
                }

                logger.LogInformation($"     Found errors: {errors.Count}");
                logger.LogInformation($"     First message: {firstMessage}");
                logger.LogInformation($"     Last message: {lastMessage}");
            }

            logErrors.Add(new LogError(errors.Count > 0, errors));
        });

        return logErrors;
    }

    private Error GetErrorFromLine(int logId, int lineNumber, ParsedLine parsedLine)
    {
        foreach (var (errorPatternId, errorPattern) in errorPatterns)
            if (errorPattern.IsMatch(parsedLine.Message))
                return new Error(
                    logId,
                    lineNumber,
                    errorPatternId,
                    parsedLine.Message,
                    parsedLine.DateTime,
                    parsedLine.RawLine
                );

        return null;
    }

    private ParsedLine ParseLine(string line)
    {
        string lineParser = @"\[(?<Date>\d{4}-\d{2}-\d{2})T(?<Time>\d{2}:\d{2}:\d{2}\.\d{1,4})Z\][ ](?<Message>.*)";

        var results = new Regex(lineParser).Matches(line);

        var dateTime = DateTime.UtcNow;
        var message = "";

        foreach (Match match in results)
        {
            message = match.Groups["Message"].Value;
            dateTime = DateTime.Parse($"{match.Groups["Date"].Value} {match.Groups["Time"].Value}").ToUniversalTime();
        }

        return new ParsedLine(line, message, dateTime);
    }
}