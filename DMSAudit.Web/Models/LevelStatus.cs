// Models/LevelStatus.cs

using DMS_Audit_MVC.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMSAudit.Web.Models;

public class LevelState
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int LevelId { get; set; }

    [ForeignKey("LevelId")]
    public required virtual Level Level { get; set; }

    [Required]
    public short Year { get; set; }

    [Required]
    public byte Month { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    [Required]
    public DateTime? ModifiedAt { get; set; }

    [Required]
    [StringLength(30)]
    public string? ModifiedBy { get; set; }
}
