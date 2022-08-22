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

    private static string LINE_PARSER =
        @"\[(?<Date>\d{4}-\d{2}-\d{2})T(?<Time>\d{2}:\d{2}:\d{2}\.\d{1,4})Z\][ ](?<Message>.*)";

    private static int ERROR_NAME_MAX_LINES = 5;
    
    public ErrorFinderService(ILogger<IErrorFinderService> logger)
    {
        this.logger = logger;
    }

    // Time regex: \[\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}\.\d{1,4}Z\]  
    private readonly Dictionary<int, Regex> errorLinePatterns = new()
    {
        { 1, new Regex(@"^[ ]{2}\d{1,3}\)[ ].*") },
    };

    private readonly Dictionary<int, Dictionary<int, Regex>> errorBrokenTestFilePatterns = new()
    {
        {
            1, new()
            {
                { 1, new Regex(@"[  ]\d{1,3}\) (?<FileNameAndPath>[a-zA-Z\/\-\.]*\.(js|ts))") }
            }
        }
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
                    logger.LogInformation($"     Error in: {error.BrokenTestName}");
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
        foreach (var (errorPatternId, errorPattern) in errorLinePatterns)
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

                errorBrokenTestFilePatterns.TryGetValue(1, out var filePatterns);

                var errorInFile = String.Empty;
                var brokenTestName = String.Empty;
                foreach (var (filePatternId, filePattern) in filePatterns)
                {
                    var matches = filePattern.Matches(parsedLine.Message);
                    if (matches.Count > 0)
                    {
                        errorInFile = matches[0].Groups["FileNameAndPath"].Value;

                        brokenTestName = GetFailingTestName(parsedLine.LineNumber, logLineCollector);

                        break;
                    }
                }

                return new Error(
                    logId,
                    lineNumber,
                    errorPatternId,
                    errorInFile,
                    brokenTestName,
                    parsedLine.Message,
                    parsedLine.DateTime,
                    parsedLine.RawLine,
                    context
                );
            }
        }

        return null;
    }

    private string GetFailingTestName(int currentErrorNameLine, Dictionary<int, ParsedLine> logLineCollector)
    {
        var testName = String.Empty;
        var tabSpace = 2;
        var expectedTabs = 3;

        int i;
        for (i = 1; i < ERROR_NAME_MAX_LINES; i++)
        {
            var currentMessage = logLineCollector[currentErrorNameLine + i].Message; 
            var leftTabs = currentMessage.TakeWhile(c => c == ' ').Count() / tabSpace;

            if (leftTabs != expectedTabs)
            {
                break;
            }

            if (i > 1)
            {
                testName += " => ";
            }
            testName += currentMessage.Trim();

            expectedTabs++;
        }

        if (i == ERROR_NAME_MAX_LINES)
        {
            logger.LogError($"Test name parsing error with: {logLineCollector[currentErrorNameLine + i]}");

            return "N/A";
        }

        return testName;
    }

    private ParsedLine ParseLine(string line, int lineNumber)
    {
        var matches = new Regex(LINE_PARSER).Matches(line);

        var dateTime = DateTime.UtcNow;
        var message = "";

        foreach (Match match in matches)
        {
            message = match.Groups["Message"].Value;
            dateTime = DateTime.Parse($"{match.Groups["Date"].Value} {match.Groups["Time"].Value}")
                .ToUniversalTime();
        }

        return new ParsedLine(line, lineNumber, message, dateTime);
    }
}