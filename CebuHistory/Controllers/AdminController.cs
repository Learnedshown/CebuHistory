using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;

namespace CebuHistory.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> users, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _users = users;
        _webHostEnvironment = webHostEnvironment;
    }

    // ── Dashboard ──────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var pendingStoriesCount = await _db.Stories.CountAsync(s => s.Status == "Pending");
        var pendingPhotosCount = await _db.Photos.CountAsync(p => p.Status == "Pending");
        var pendingDocumentsCount = await _db.Documents.CountAsync(d => d.Status == "Pending");
        var publishedCount = await _db.Stories.CountAsync(s => s.Status == "Published");
        var rejectedCount = await _db.Stories.CountAsync(s => s.Status == "Rejected");

        var recentSubmissions = await _db.Stories
            .Where(s => s.Status == "Pending")
            .OrderByDescending(s => s.SubmittedAt)
            .Take(10)
            .Include(s => s.SubmittedByUser)
            .ToListAsync();

        var vm = new AdminDashboardViewModel
        {
            TotalStories = await _db.Stories.CountAsync(),
            TotalPhotos = await _db.Photos.CountAsync(),
            TotalDocuments = await _db.Documents.CountAsync(),
            TotalUsers = await _users.Users.CountAsync(),
            RecentStories = await _db.Stories.OrderByDescending(s => s.CreatedAt).Take(5).ToListAsync(),
            PendingStories = pendingStoriesCount,
            PendingPhotos = pendingPhotosCount,
            PendingDocuments = pendingDocumentsCount,
            ApprovedStories = publishedCount,
            RejectedStories = rejectedCount,
            RecentSubmissions = recentSubmissions
        };
        return View(vm);
    }

    // ── Stories ────────────────────────────────────────────────
    public async Task<IActionResult> Stories() =>
        View(await _db.Stories.OrderByDescending(s => s.CreatedAt).ToListAsync());

    [HttpGet]
    public IActionResult CreateStory() => View(new StoryFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateStory(StoryFormViewModel vm, IFormFile? CoverImageFile)
    {

        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return View(vm);
        }

        var coverImageUrl = vm.CoverImage;
        if (CoverImageFile != null && CoverImageFile.Length > 0)
        {
            coverImageUrl = await SaveUploadedFile(CoverImageFile, "stories");
        }

        var story = new Story
        {
            Title = vm.Title,
            Slug = GenerateSlug(vm.Title),
            Excerpt = vm.Excerpt,
            Body = vm.Body,
            Author = vm.Author,
            Era = vm.Era,
            Category = vm.Category,
            CoverImage = coverImageUrl ?? "",
            IsFeatured = vm.IsFeatured,
            Status = vm.Status,
            Tags = vm.Tags,
            ReadingTime = vm.ReadingTime,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
        };

        _db.Stories.Add(story);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Story published successfully.";
        return RedirectToAction(nameof(Stories));
    }

    [HttpGet]
    public async Task<IActionResult> EditStory(int id)
    {
        var s = await _db.Stories.FindAsync(id);
        if (s == null) return NotFound();
        return View(new StoryFormViewModel
        {
            Id = s.Id,
            Title = s.Title,
            Excerpt = s.Excerpt,
            Body = s.Body,
            Author = s.Author,
            Era = s.Era,
            Category = s.Category,
            CoverImage = s.CoverImage,
            IsFeatured = s.IsFeatured,
            Status = s.Status,
            Tags = s.Tags,
            ReadingTime = s.ReadingTime,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStory(int id, StoryFormViewModel vm, IFormFile? CoverImageFile)
    {
        if (!ModelState.IsValid) return View(vm);
        var s = await _db.Stories.FindAsync(id);
        if (s == null) return NotFound();

        if (CoverImageFile != null && CoverImageFile.Length > 0)
        {
            s.CoverImage = await SaveUploadedFile(CoverImageFile, "stories");
        }
        else if (!string.IsNullOrEmpty(vm.CoverImage))
        {
            s.CoverImage = vm.CoverImage;
        }

        s.Title = vm.Title;
        s.Excerpt = vm.Excerpt;
        s.Body = vm.Body;
        s.Author = vm.Author;
        s.Era = vm.Era;
        s.Category = vm.Category;
        s.IsFeatured = vm.IsFeatured;
        s.Status = vm.Status;
        s.Tags = vm.Tags;
        s.ReadingTime = vm.ReadingTime;
        s.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Story updated.";
        return RedirectToAction(nameof(Stories));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStory(int id)
    {
        var s = await _db.Stories.FindAsync(id);
        if (s != null) { _db.Stories.Remove(s); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Story deleted.";
        return RedirectToAction(nameof(Stories));
    }

    // ── Photos ─────────────────────────────────────────────
    public async Task<IActionResult> Photos() =>
        View(await _db.Photos.OrderByDescending(p => p.UploadedAt).ToListAsync());

    [HttpGet]
    public IActionResult CreatePhoto() => View(new PhotoFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePhoto(PhotoFormViewModel vm, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return View(vm);
        }

        var imageUrl = vm.ImageUrl;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            imageUrl = await SaveUploadedFile(ImageFile, "photos");
        }

        _db.Photos.Add(new Photo
        {
            Title = vm.Title,
            Description = vm.Description,
            ImageUrl = imageUrl ?? "",
            Era = vm.Era,
            Year = vm.Year,
            Source = vm.Source,
            Location = vm.Location,
            Photographer = vm.Photographer,
            IsPublicDomain = vm.IsPublicDomain,
            UploadedAt = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Photo added.";
        return RedirectToAction(nameof(Photos));
    }

    [HttpGet]
    public async Task<IActionResult> EditPhoto(int id)
    {
        var p = await _db.Photos.FindAsync(id);
        if (p == null) return NotFound();
        return View(new PhotoFormViewModel
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            ImageUrl = p.ImageUrl,
            Era = p.Era,
            Year = p.Year,
            Source = p.Source,
            Location = p.Location,
            Photographer = p.Photographer,
            IsPublicDomain = p.IsPublicDomain,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPhoto(int id, PhotoFormViewModel vm, IFormFile? ImageFile)
    {

        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return View(vm);
        }
        var p = await _db.Photos.FindAsync(id);
        if (p == null) return NotFound();

        if (ImageFile != null && ImageFile.Length > 0)
        {
            p.ImageUrl = await SaveUploadedFile(ImageFile, "photos");
        }
        else if (!string.IsNullOrEmpty(vm.ImageUrl))
        {
            p.ImageUrl = vm.ImageUrl;
        }

        p.Title = vm.Title;
        p.Description = vm.Description;
        p.Era = vm.Era;
        p.Year = vm.Year;
        p.Source = vm.Source;
        p.Location = vm.Location;
        p.Photographer = vm.Photographer;
        p.IsPublicDomain = vm.IsPublicDomain;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Photo updated.";
        return RedirectToAction(nameof(Photos));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        var p = await _db.Photos.FindAsync(id);
        if (p != null) { _db.Photos.Remove(p); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Photo deleted.";
        return RedirectToAction(nameof(Photos));
    }

    // ── Documents ──────────────────────────────────────────
    public async Task<IActionResult> Documents() =>
        View(await _db.Documents.OrderByDescending(d => d.UploadedAt).ToListAsync());

    [HttpGet]
    public IActionResult CreateDocument() => View(new DocumentFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDocument(DocumentFormViewModel vm, IFormFile? DocumentFile, IFormFile? ThumbnailFile)
    {

        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return View(vm);
        }

        var documentUrl = vm.DocumentUrl;
        if (DocumentFile != null && DocumentFile.Length > 0)
        {
            documentUrl = await SaveUploadedFile(DocumentFile, "documents");
        }

        var thumbnailUrl = vm.ThumbnailUrl;
        if (ThumbnailFile != null && ThumbnailFile.Length > 0)
        {
            thumbnailUrl = await SaveUploadedFile(ThumbnailFile, "documents/thumbnails");
        }

        _db.Documents.Add(new HistoricalDocument
        {
            Title = vm.Title,
            Description = vm.Description,
            DocumentUrl = documentUrl ?? "",
            ThumbnailUrl = thumbnailUrl,
            Era = vm.Era,
            Year = vm.Year,
            Source = vm.Source,
            DocumentType = vm.DocumentType,
            UploadedAt = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync();
        TempData["Success"] = "Document added.";
        return RedirectToAction(nameof(Documents));
    }

    [HttpGet]
    public async Task<IActionResult> EditDocument(int id)
    {
        var d = await _db.Documents.FindAsync(id);
        if (d == null) return NotFound();
        return View(new DocumentFormViewModel
        {
            Id = d.Id,
            Title = d.Title,
            Description = d.Description,
            DocumentUrl = d.DocumentUrl,
            ThumbnailUrl = d.ThumbnailUrl,
            Era = d.Era,
            Year = d.Year,
            Source = d.Source,
            DocumentType = d.DocumentType,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDocument(int id, DocumentFormViewModel vm, IFormFile? DocumentFile, IFormFile? ThumbnailFile)
    {

        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return View(vm);
        }
        var d = await _db.Documents.FindAsync(id);
        if (d == null) return NotFound();

        if (DocumentFile != null && DocumentFile.Length > 0)
        {
            d.DocumentUrl = await SaveUploadedFile(DocumentFile, "documents");
        }
        else if (!string.IsNullOrEmpty(vm.DocumentUrl))
        {
            d.DocumentUrl = vm.DocumentUrl;
        }

        if (ThumbnailFile != null && ThumbnailFile.Length > 0)
        {
            d.ThumbnailUrl = await SaveUploadedFile(ThumbnailFile, "documents/thumbnails");
        }
        else if (!string.IsNullOrEmpty(vm.ThumbnailUrl))
        {
            d.ThumbnailUrl = vm.ThumbnailUrl;
        }

        d.Title = vm.Title;
        d.Description = vm.Description;
        d.Era = vm.Era;
        d.Year = vm.Year;
        d.Source = vm.Source;
        d.DocumentType = vm.DocumentType;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Document updated.";
        return RedirectToAction(nameof(Documents));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var d = await _db.Documents.FindAsync(id);
        if (d != null) { _db.Documents.Remove(d); await _db.SaveChangesAsync(); }
        TempData["Success"] = "Document deleted.";
        return RedirectToAction(nameof(Documents));
    }

    // ── Users Management ──────────────────────────────────────
    public async Task<IActionResult> Users()
    {
        var users = await _users.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
        var result = new List<(ApplicationUser User, IList<string> Roles)>();
        foreach (var u in users)
            result.Add((u, await _users.GetRolesAsync(u)));
        ViewBag.UserRoles = result;
        return View(users);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAdmin(string userId)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user == null) return NotFound();
        if (await _users.IsInRoleAsync(user, "Admin"))
            await _users.RemoveFromRoleAsync(user, "Admin");
        else
            await _users.AddToRoleAsync(user, "Admin");
        TempData["Success"] = "User role updated.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleUserStatus(string userId)
    {
        var user = await _users.FindByIdAsync(userId);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        await _users.UpdateAsync(user);

        TempData["Success"] = $"User {user.Email} is now {(user.IsActive ? "active" : "disabled")}";
        return RedirectToAction(nameof(Users));
    }

    // ── Pending Stories (Approve/Reject) ──────────────────────
    public async Task<IActionResult> PendingStories()
    {
        var pendingStories = await _db.Stories
            .Where(s => s.Status == "Pending")
            .OrderByDescending(s => s.SubmittedAt)
            .Include(s => s.SubmittedByUser)
            .ToListAsync();

        return View(pendingStories);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveStory(int id)
    {
        var story = await _db.Stories.FindAsync(id);
        if (story == null) return NotFound();

        story.Status = "Published";
        story.PublishedAt = DateTime.UtcNow;
        story.ApprovedAt = DateTime.UtcNow;
        story.ApprovedByUserId = _users.GetUserId(User);

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Story '{story.Title}' approved and published.";
        return RedirectToAction(nameof(PendingStories));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectStory(int id, string rejectionReason)
    {
        var story = await _db.Stories.FindAsync(id);
        if (story == null) return NotFound();

        story.Status = "Rejected";
        story.RejectionReason = rejectionReason;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Story '{story.Title}' rejected.";
        return RedirectToAction(nameof(PendingStories));
    }

    // ── All Stories (Complete Management) ─────────────────────
    public async Task<IActionResult> AllStories()
    {
        var stories = await _db.Stories
            .OrderByDescending(s => s.CreatedAt)
            .Include(s => s.SubmittedByUser)
            .ToListAsync();

        return View(stories);
    }

    // ── Pending Photos (Approve/Reject) ──────────────────────
    public async Task<IActionResult> PendingPhotos()
    {
        var pendingPhotos = await _db.Photos
            .Where(p => p.Status == "Pending")
            .OrderByDescending(p => p.SubmittedAt)
            .Include(p => p.SubmittedByUser)
            .ToListAsync();
        return View(pendingPhotos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApprovePhoto(int id)
    {
        var photo = await _db.Photos.FindAsync(id);
        if (photo == null) return NotFound();

        photo.Status = "Published";
        photo.ApprovedAt = DateTime.UtcNow;
        photo.ApprovedByUserId = _users.GetUserId(User);

        await _db.SaveChangesAsync();
        TempData["Success"] = $"Photo '{photo.Title}' approved and published.";
        return RedirectToAction(nameof(PendingPhotos));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectPhoto(int id, string rejectionReason)
    {
        var photo = await _db.Photos.FindAsync(id);
        if (photo == null) return NotFound();

        photo.Status = "Rejected";
        photo.RejectionReason = rejectionReason;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Photo '{photo.Title}' rejected.";
        return RedirectToAction(nameof(PendingPhotos));
    }

    // ── Pending Documents (Approve/Reject) ──────────────────────
    public async Task<IActionResult> PendingDocuments()
    {
        var pendingDocs = await _db.Documents
            .Where(d => d.Status == "Pending")
            .OrderByDescending(d => d.SubmittedAt)
            .Include(d => d.SubmittedByUser)
            .ToListAsync();
        return View(pendingDocs);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveDocument(int id)
    {
        var doc = await _db.Documents.FindAsync(id);
        if (doc == null) return NotFound();

        doc.Status = "Published";
        doc.ApprovedAt = DateTime.UtcNow;
        doc.ApprovedByUserId = _users.GetUserId(User);

        await _db.SaveChangesAsync();
        TempData["Success"] = $"Document '{doc.Title}' approved and published.";
        return RedirectToAction(nameof(PendingDocuments));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectDocument(int id, string rejectionReason)
    {
        var doc = await _db.Documents.FindAsync(id);
        if (doc == null) return NotFound();

        doc.Status = "Rejected";
        doc.RejectionReason = rejectionReason;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Document '{doc.Title}' rejected.";
        return RedirectToAction(nameof(PendingDocuments));
    }

    // ── File Upload Helper ────────────────────────────────────
    private async Task<string> SaveUploadedFile(IFormFile file, string subFolder)
    {
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", subFolder);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/uploads/{subFolder}/{uniqueFileName}";
    }

    // ── Helpers ────────────────────────────────────────────────
    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("&", "and")
            .Replace("–", "-")
            .Replace("—", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-").Trim('-');
        return slug + "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
    }
}