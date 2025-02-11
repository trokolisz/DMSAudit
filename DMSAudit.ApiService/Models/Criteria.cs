// Models/Criterias.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DMSAudit.ApiService.Models;

public class Criteria
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(30)]    
    public required string Name { get; set; }

    [StringLength(5000)]
    public required string Description { get; set; }

    [StringLength(500)]
    public required string Group { get; set; }
    
    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<CriteriaState> CriteriaStates { get; set; } = [];
    public virtual ICollection<Level> Levels { get; set; } = [];
}

public class CriteriaOnly
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Group { get; set; }
}
public class CriteriaCreate
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Group { get; set; }
    public required string[] LevelDescriptions { get; set; } = new string[5];
}

