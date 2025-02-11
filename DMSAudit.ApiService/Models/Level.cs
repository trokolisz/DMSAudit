// Models/Levels.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DMSAudit.ApiService.Models;
public class Level
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CriteriaId { get; set; }

    [ForeignKey(nameof(CriteriaId))]
    [JsonIgnore]
    public virtual Criteria Criteria { get; set; } = null!;

    [Required]
    public short Level_ { get; set; }

    [StringLength(500)]
    public required string Description { get; set; }
    [JsonIgnore]
    public ICollection<LevelState> LevelStates { get; set; } = [];
    [JsonIgnore]
    public ICollection<Project> Projects { get; set; } = [];
}
