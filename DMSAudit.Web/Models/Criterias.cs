// Models/Criterias.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMSAudit.Web.Models;

public class Criteria
{
    [Key]
    public int ID { get; set; }

    [Required]
    [StringLength(30)]
    public required string Name { get; set; }

    [StringLength(5000)]
    public required string Description { get; set; }

    [StringLength(500)]
    public required string Group { get; set; }
}

