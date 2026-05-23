using System.ComponentModel.DataAnnotations;

namespace CebuHistory.Models;

// ── Public-facing ──────────────────────────────────────────────
public class HomeViewModel
{
    public List<Story> FeaturedStories { get; set; } = new();
    public List<Story> LatestStories { get; set; } = new();
    public List<Photo> RecentPhotos { get; set; } = new();
    public List<HistoricalDocument> RecentDocuments { get; set; } = new();
    public List<TimelineEvent> TimelineEvents { get; set; } = new();
}

public class StoriesViewModel
{
    public List<Story> Stories { get; set; } = new();
    public List<string> Eras { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public string? ActiveEra { get; set; }
    public string? ActiveCategory { get; set; }
    public string? SearchQuery { get; set; }
}

public class GalleryViewModel
{
    public List<Photo> Photos { get; set; } = new();
    public List<string> Eras { get; set; } = new();
    public string? ActiveEra { get; set; }
}

public class DocumentsViewModel
{
    public List<HistoricalDocument> Documents { get; set; } = new();
    public List<string> Types { get; set; } = new();
    public string? ActiveType { get; set; }
}

// ── Account ────────────────────────────────────────────────────
public class LoginViewModel
{
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required, MaxLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, MaxLength(100)] public string LastName { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required, MinLength(6), DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    [Compare("Password"), DataType(DataType.Password)] public string ConfirmPassword { get; set; } = string.Empty;
}

// ── Admin ──────────────────────────────────────────────────────
public class StoryFormViewModel
{
    public int Id { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [MaxLength(500)] public string Excerpt { get; set; } = string.Empty;
    [Required] public string Body { get; set; } = string.Empty;
    [MaxLength(100)] public string Author { get; set; } = string.Empty;
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    [MaxLength(100)] public string Category { get; set; } = string.Empty;
    public string CoverImage { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public string Status { get; set; } = "Published";
    public string Tags { get; set; } = string.Empty;
    public int ReadingTime { get; set; } = 5;
}

public class PhotoFormViewModel
{
    public int Id { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required] public string ImageUrl { get; set; } = string.Empty;
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    public int? Year { get; set; }
    [MaxLength(200)] public string Source { get; set; } = string.Empty;
    [MaxLength(200)] public string Location { get; set; } = string.Empty;
    public string? Photographer { get; set; }
    public bool IsPublicDomain { get; set; } = true;
}

public class DocumentFormViewModel
{
    public int Id { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [Required] public string DocumentUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    [MaxLength(100)] public string Era { get; set; } = string.Empty;
    public int? Year { get; set; }
    [MaxLength(200)] public string Source { get; set; } = string.Empty;
    [MaxLength(50)] public string DocumentType { get; set; } = string.Empty;
}

public class AdminDashboardViewModel
{
    public int TotalStories { get; set; }
    public int TotalPhotos { get; set; }
    public int TotalDocuments { get; set; }
    public int TotalUsers { get; set; }
    public List<Story> RecentStories { get; set; } = new List<Story>();
    public int PendingStories { get; set; }
    public int ApprovedStories { get; set; }
    public int RejectedStories { get; set; }
    public List<Story> RecentSubmissions { get; set; } = new List<Story>();  // Initialize with empty list
}

// ==========================================
// ADD THESE TO YOUR EXISTING ViewModels.cs
// ==========================================

public class SubmitStoryViewModel
{
    public string Title { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string Body { get; set; } = "";
    public string Author { get; set; } = "";
    public string Era { get; set; } = "";
    public string Category { get; set; } = "";
    public string CoverImage { get; set; } = "";
    public string Tags { get; set; } = "";
    public string SubmitterName { get; set; } = "";
}

public class EditStoryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Excerpt { get; set; } = "";
    public string Body { get; set; } = "";
    public string Author { get; set; } = "";
    public string Era { get; set; } = "";
    public string Category { get; set; } = "";
    public string CoverImage { get; set; } = "";
    public string Tags { get; set; } = "";
    public string? RejectionReason { get; set; }
}