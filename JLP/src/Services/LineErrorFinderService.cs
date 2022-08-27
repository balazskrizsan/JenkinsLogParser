using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JLP.Entities;
using JLP.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JLP.Services;

public class LineErrorFinderService : ILineErrorFinderService
{
    private static string LINE_PARSER =
        @"\[(?<Date>\d{4}-\d{2}-\d{2})T(?<Time>\d{2}:\d{2}:\d{2}\.\d{1,4})Z\][ ](?<Message>.*)";

    private static int ERROR_NAME_MAX_LINES = 5;

    private static readonly Dictionary<int, Regex> ERROR_LINE_PATTERNS = new()
    {
        { 1, new Regex(@"^[ ]{2}\d{1,3}\)[ ].*") },
    };

    private readonly Dictionary<int, Dictionary<int, Regex>> BROKEN_TEST_FILES_PATTERNS = new()
    {
        {
            1, new()
            {
                { 1, new Regex(@"[  ]\d{1,3}\) (?<FileNameAndPath>[a-zA-Z\/\-\.]*\.(js|ts))") }
            }
        }
    };

    private ILogger<LineErrorFinderService> logger;

    public LineErrorFinderService(ILogger<LineErrorFinderService> logger)
    {
        this.logger = logger;
    }

    public Error SearchError(
        int logId,
        int lineNumber,
        ParsedLine parsedLine,
        Dictionary<int, ParsedLine> logLineCollector
    )
    {
        foreach (var (errorPatternId, errorPattern) in ERROR_LINE_PATTERNS)
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

                BROKEN_TEST_FILES_PATTERNS.TryGetValue(1, out var filePatterns);

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
}