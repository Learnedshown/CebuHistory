using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuHistory.Models;

public class Photo
{
    [Key] public int Id { get; set; }

    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required] public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    public int? Year { get; set; }
    [MaxLength(200)] public string Source { get; set; } = string.Empty;
    [MaxLength(200)] public string Location { get; set; } = string.Empty;
    public string? Photographer { get; set; }
    public bool IsPublicDomain { get; set; } = true;
    public string? UploadedById { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UploadedById")]
    public virtual ApplicationUser? UploadedBy { get; set; }
}