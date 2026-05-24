using CebuHistory.Data;
using CebuHistory.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CebuHistory.Controllers;

public class SubmitStoryController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;  // ADDED

    public SubmitStoryController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)  // UPDATED
    {
        _db = db;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;  // ADDED
    }

    // ==========================================
    // SUBMIT NEW STORY (Anyone can submit)
    // ==========================================
    [HttpGet]
    public IActionResult Index()
    {
        return View(new SubmitStoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SubmitStoryViewModel vm, IFormFile? ImageFile)  // ADDED ImageFile parameter
    {
        if (!ModelState.IsValid) {
            TempData["Errors"] = string.Join("|",ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage));
            return View(vm);
        }


        var userId = User.Identity?.IsAuthenticated == true
            ? _userManager.GetUserId(User)
            : null;

        var userName = User.Identity?.IsAuthenticated == true
            ? User.Identity.Name ?? "Anonymous"
            : (string.IsNullOrEmpty(vm.SubmitterName) ? "Anonymous" : vm.SubmitterName);

        // Handle file upload - if user uploaded an image, save it
        var coverImageUrl = vm.CoverImage;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            coverImageUrl = await SaveUploadedFile(ImageFile, "stories");
        }

        var story = new Story
        {
            Title = vm.Title,
            Slug = GenerateSlug(vm.Title),
            Excerpt = vm.Excerpt,
            Body = vm.Body,
            Author = string.IsNullOrEmpty(vm.Author) ? userName : vm.Author,
            Era = vm.Era,
            Category = vm.Category,
            CoverImage = coverImageUrl ?? string.Empty,  // UPDATED to use uploaded file or URL
            Tags = vm.Tags ?? string.Empty,
            Status = "Pending",
            SubmittedByUserId = userId,
            SubmittedByName = userName,
            SubmittedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            ReadingTime = CalculateReadingTime(vm.Body)
        };

        _db.Stories.Add(story);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thank you! Your story has been submitted for review.";
        return RedirectToAction(nameof(Index));
    }

    // ==========================================
    // MY STORIES (Logged in users only)
    // ==========================================
    [Authorize]
    public async Task<IActionResult> MyStories()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var stories = await _db.Stories
            .Where(s => s.SubmittedByUserId == userId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync();

        return View(stories);
    }

    // ==========================================
    // EDIT MY STORY
    // ==========================================
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditStory(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var story = await _db.Stories.FirstOrDefaultAsync(s => s.Id == id && s.SubmittedByUserId == userId);

        if (story == null)
            return NotFound();

        if (story.Status != "Pending" && story.Status != "Rejected")
        {
            TempData["Error"] = "You cannot edit an approved story. Contact admin for changes.";
            return RedirectToAction(nameof(MyStories));
        }

        var vm = new EditStoryViewModel
        {
            Id = story.Id,
            Title = story.Title,
            Excerpt = story.Excerpt,
            Body = story.Body,
            Author = story.Author,
            Era = story.Era,
            Category = story.Category,
            CoverImage = story.CoverImage,
            Tags = story.Tags,
            RejectionReason = story.RejectionReason
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStory(int id, EditStoryViewModel vm, IFormFile? ImageFile)  // ADDED ImageFile parameter
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var story = await _db.Stories.FirstOrDefaultAsync(s => s.Id == id && s.SubmittedByUserId == userId);

        if (story == null)
            return NotFound();

        // Handle new image upload if user provided one
        if (ImageFile != null && ImageFile.Length > 0)
        {
            story.CoverImage = await SaveUploadedFile(ImageFile, "stories");
        }
        else if (!string.IsNullOrEmpty(vm.CoverImage))
        {
            story.CoverImage = vm.CoverImage;
        }

        story.Title = vm.Title;
        story.Slug = GenerateSlug(vm.Title);
        story.Excerpt = vm.Excerpt;
        story.Body = vm.Body;
        story.Author = vm.Author;
        story.Era = vm.Era;
        story.Category = vm.Category;
        story.Tags = vm.Tags ?? string.Empty;
        story.Status = "Pending";
        story.UpdatedAt = DateTime.UtcNow;
        story.RejectionReason = null;
        story.ReadingTime = CalculateReadingTime(vm.Body);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Your story has been updated and submitted for re-review.";
        return RedirectToAction(nameof(MyStories));
    }

    // ==========================================
    // DELETE MY STORY
    // ==========================================
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteStory(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var story = await _db.Stories.FirstOrDefaultAsync(s => s.Id == id && s.SubmittedByUserId == userId);

        if (story != null)
        {
            _db.Stories.Remove(story);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Your story has been deleted.";
        }

        return RedirectToAction(nameof(MyStories));
    }

    // ==========================================
    // FILE UPLOAD HELPER
    // ==========================================
    private async Task<string> SaveUploadedFile(IFormFile file, string subFolder)
    {
        // Create directory if not exists
        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", subFolder);
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        // Generate unique filename
        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save file
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/uploads/{subFolder}/{uniqueFileName}";
    }

    // ==========================================
    // HELPERS
    // ==========================================
    private static string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title))
            return "untitled-" + DateTime.UtcNow.Ticks.ToString()[^6..];

        var slug = title.ToLower().Replace(" ", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        return slug + "-" + DateTime.UtcNow.Ticks.ToString()[^6..];
    }

    private static int CalculateReadingTime(string content)
    {
        if (string.IsNullOrEmpty(content))
            return 1;

        var words = content.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return (int)Math.Max(1, Math.Ceiling(words.Length / 200.0));
    }
}