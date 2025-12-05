using System.ComponentModel.DataAnnotations;

namespace ClaimTrackerBlazor.Contracts;

public class Claim
{
    public int ClaimId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ClaimNumber { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Policy Holder Name")]
    [MaxLength(100)]
    public string PolicyHolderName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of Incident")]
    public DateTime DateOfIncident { get; set; }

    [Required]
    [Display(Name = "Date Filed")]
    public DateTime DateFiled { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Claim amount must be greater than zero.")]
    public decimal ClaimAmount { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}
