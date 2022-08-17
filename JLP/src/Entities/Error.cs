using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JLP.Entities;

[Table("errors")]
[Keyless]
public class Error
{
    [Column("log_id")] public int LogId { get; set; }
    [Column("line_number")] public int LineNumber { get; set; }
    [Column("error_pattern_id")] public int ErrorPatternId { get; set; }
    [Column("message")] public string Message { get; set; }
    [Column("error_time")] public DateTime ErrorTime { get; set; }
    [Column("raw_line")] public string RawLine { get; set; }

    public Error(
        int logId,
        int lineNumber,
        int errorPatternId,
        string message,
        DateTime errorTime,
        string rawLine
    )
    {
        LogId = logId;
        LineNumber = lineNumber;
        ErrorPatternId = errorPatternId;
        Message = message;
        ErrorTime = errorTime;
        RawLine = rawLine;
    }
}