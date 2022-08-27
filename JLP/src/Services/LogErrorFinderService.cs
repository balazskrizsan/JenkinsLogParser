using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JLP.Entities;
using JLP.ValueObjects;
using Microsoft.Extensions.Logging;

namespace JLP.Services;

public class LogErrorFinderService : ILogErrorFinderService
{
    private static string LINE_PARSER =
        @"\[(?<Date>\d{4}-\d{2}-\d{2})T(?<Time>\d{2}:\d{2}:\d{2}\.\d{1,4})Z\][ ](?<Message>.*)";

    private ILogger<ILogErrorFinderService> logger;
    private readonly ILineErrorFinderService lineErrorFinderService;

    public LogErrorFinderService(
        ILogger<ILogErrorFinderService> logger,
        ILineErrorFinderService lineErrorFinderService
    )
    {
        this.lineErrorFinderService = lineErrorFinderService;
        this.logger = logger;
    }

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
                    var parsedLine = ParseRowLine(line, lineNumber);
                    logLineCollector.Add(parsedLine.LineNumber, parsedLine);
                }
            }

            foreach (var (lineNumber, parsedLine) in logLineCollector)
            {
                var error = lineErrorFinderService.SearchError(log.Id ?? 0, lineNumber, parsedLine, logLineCollector);
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

    private ParsedLine ParseRowLine(string line, int lineNumber)
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