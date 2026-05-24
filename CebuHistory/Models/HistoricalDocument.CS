using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuHistory.Models;

public class HistoricalDocument
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DocumentUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    public int? Year { get; set; }
    [MaxLength(200)] public string Source { get; set; } = string.Empty;
    [MaxLength(50)] public string DocumentType { get; set; } = string.Empty;
    public string? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UploadedById")]
    public virtual ApplicationUser? UploadedBy { get; set; }


    // Add these properties to your existing HistoricalDocument class

    // For user submissions
    public string Status { get; set; } = "Pending"; // Pending, Published, Rejected, Draft
    public string? SubmittedByUserId { get; set; }
    public string? SubmittedByName { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string? RejectionReason { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    [ForeignKey("SubmittedByUserId")]
    public virtual ApplicationUser? SubmittedByUser { get; set; }
}