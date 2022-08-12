using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JLP.Entities;

[Table("errors")]
[Keyless]
public class Error
{
    public Error(int logId, string originalLine, int lineNumber, int errorPatternId)
    {
        LogId = logId;
        OriginalLine = originalLine;
        LineNumber = lineNumber;
        ErrorPatternId = errorPatternId;
    }

    [Column("log_id")] public int LogId { get; set; }
    [Column("original_line")] public string OriginalLine { get; set; }
    [Column("line_number")] public int LineNumber { get; set; }
    [Column("error_pattern_id")] public int ErrorPatternId { get; set; }
}