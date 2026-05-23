using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuHistory.Models;

public class Bookmark
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public int StoryId { get; set; }
    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")] public virtual ApplicationUser? User { get; set; }
    [ForeignKey("StoryId")] public virtual Story? Story { get; set; }
}