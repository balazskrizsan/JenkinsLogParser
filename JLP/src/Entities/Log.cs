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
}