using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuHistory.Models;

public class Story
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [Required, MaxLength(200)] public string Slug { get; set; } = string.Empty;
    [MaxLength(500)] public string Excerpt { get; set; } = string.Empty;
    [Required] public string Body { get; set; } = string.Empty;
    [MaxLength(100)] public string Author { get; set; } = string.Empty;
    public string? AuthorId { get; set; }
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    [MaxLength(100)] public string Category { get; set; } = string.Empty;
    public string? CoverImage { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public bool IsFeatured { get; set; }
    public int Views { get; set; } = 0;
    public int ReadingTime { get; set; }
    public string Status { get; set; } = "Pending";  // Changed: Pending, Published, Rejected, Draft
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string Tags { get; set; } = string.Empty;

    [ForeignKey("AuthorId")]
    public virtual ApplicationUser? AuthorUser { get; set; }

    // ==========================================
    // NEW FIELDS FOR USER SUBMISSIONS
    // ==========================================

    // Who submitted the story (for non-logged in users, this stores their name)
    public string? SubmittedByName { get; set; }

    // User ID if logged in when submitting
    public string? SubmittedByUserId { get; set; }

    // When the story was submitted
    public DateTime? SubmittedAt { get; set; }

    // Why it was rejected (admin provides reason)
    public string? RejectionReason { get; set; }

    // Who approved it (admin user ID)
    public string? ApprovedByUserId { get; set; }

    // When it was approved
    public DateTime? ApprovedAt { get; set; }

    // Navigation property for the user who submitted
    [ForeignKey("SubmittedByUserId")]
    public virtual ApplicationUser? SubmittedByUser { get; set; }
}