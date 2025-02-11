// Models/Levels.cs
using DMSAudit.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DMS_Audit_MVC.Models;
public class Level
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CriteriaID { get; set; }

    [ForeignKey("CriteriaID")]
    public required virtual Criteria Criteria { get; set; }

    [Required]
    public short Level_ { get; set; }

    [StringLength(500)]
    public required string Description { get; set; }
}
