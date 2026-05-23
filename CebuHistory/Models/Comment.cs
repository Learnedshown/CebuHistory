using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuHistory.Models;

public class Comment
{
    [Key] public int Id { get; set; }
    [Required] public string Content { get; set; } = string.Empty;
    [Required] public int StoryId { get; set; }
    [Required] public string UserId { get; set; } = string.Empty;
    public int? ParentCommentId { get; set; }
    public bool IsApproved { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("StoryId")] public virtual Story? Story { get; set; }
    [ForeignKey("UserId")] public virtual ApplicationUser? User { get; set; }
}