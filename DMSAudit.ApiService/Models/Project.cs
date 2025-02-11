// Models/Projects.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DMSAudit.ApiService.Models;


public class Project
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int LevelId { get; set; }

    [ForeignKey("LevelId")]
    [JsonIgnore]
    public required virtual Level Level { get; set; }

    [Required]
    [StringLength(255)]
    public required string AmAzon { get; set; }

    [Required]
    public DateTime AddedDate { get; set; }

    [Required]
    [StringLength(30)]
    public required string AddedBy { get; set; }

    [Required]
    public short? State { get; set; }

    public bool Deleted { get; set; } = false;

    public DateTime? DeletedDate { get; set; }

    [StringLength(30)]
    public string? DeletedBy { get; set; }
}
