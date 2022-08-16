using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JLP.Entities;

[Table("logs")]
public class Log
{
    [Column("id")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int? Id { get; set; }

    [Column("raw_log")] public string RawLog { get; set; }
    [Column("log_external_id")] public int LogExternalId { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; }
}