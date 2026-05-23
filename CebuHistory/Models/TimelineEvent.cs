using System.ComponentModel.DataAnnotations;

namespace CebuHistory.Models;

public class TimelineEvent
{
    [Key] public int Id { get; set; }
    [Required] public int Year { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int Importance { get; set; } = 5;
}