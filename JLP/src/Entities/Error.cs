using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace JLP.Entities;

[Table("errors")]
[Keyless]
public class Error
{
    public Error(int logId, string originalLine, int lineNumber, int patternId)
    {
        LogId = logId;
        OriginalLine = originalLine;
        LineNumber = lineNumber;
        PatternId = patternId;
    }

    [Column("log_id")] public int LogId { get; set; }
    [Column("original_line")] public string OriginalLine { get; set; }
    [Column("line_number")] public int LineNumber { get; set; }
    [Column("pattern_id")] public int PatternId { get; set; }
}