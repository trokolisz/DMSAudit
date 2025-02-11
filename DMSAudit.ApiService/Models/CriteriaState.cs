// Models/CriteriaStatus.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMSAudit.ApiService.Models;
public class CriteriaState
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required int CriteriaId { get; set; }

    [ForeignKey(nameof(CriteriaId))]
    public virtual Criteria? Criteria { get; set; }

    [Required]
    public required short Year { get; set; }

    [Required]
    public required byte Month { get; set; }

    [Required]
    public required short CurrentLvl { get; set; }

    [Required]
    public float Progress { get; set; } = 0f;

    public DateTime? ModifiedDate { get; set; }


    [StringLength(30)]
    public string? ModifiedBy { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }

    public bool Closed { get; set; } = false;

    public DateTime? ClosedDate { get; set; }

    [StringLength(30)]
    public string? ClosedBy { get; set; }

    [StringLength(500)]
    public string? ClosingComment { get; set; }
}
