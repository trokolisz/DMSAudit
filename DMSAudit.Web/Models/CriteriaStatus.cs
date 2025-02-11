// Models/CriteriaStatus.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMSAudit.Web.Models;
public class CombinedCriteriaData
{
    public required Criteria Criteria { get; set; }
    public required CriteriaState CriteriaState { get; set; }
}
public class CriteriaState
{
    [Key]
    public int ID { get; set; }

    [Required]
    public required int CriteriaID { get; set; }

    [ForeignKey("CriteriaID")]
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
