using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;

namespace CebuHistory.Controllers;

[Authorize]
public class SubmitDocumentController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SubmitDocumentController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SubmitDocumentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SubmitDocumentViewModel vm, IFormFile? DocumentFile, IFormFile? ThumbnailFile)
    {
        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(vm);
        }

        var userId = _userManager.GetUserId(User);
        var userName = User.Identity?.Name ?? "Anonymous";

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

        var document = new HistoricalDocument
        {
            Title = vm.Title,
            Description = vm.Description,
            DocumentUrl = documentUrl ?? string.Empty,
            ThumbnailUrl = thumbnailUrl,
            Era = vm.Era,
            Year = vm.Year,
            Source = vm.Source,
            DocumentType = vm.DocumentType,
            Status = "Pending",
            SubmittedByUserId = userId,
            SubmittedByName = userName,
            SubmittedAt = DateTime.UtcNow,
            UploadedAt = DateTime.UtcNow,
        };

        _db.Documents.Add(document);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thank you! Your document has been submitted for review.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> MyDocuments()
    {
        var userId = _userManager.GetUserId(User);
        var documents = await _db.Documents
            .Where(d => d.SubmittedByUserId == userId)
            .OrderByDescending(d => d.SubmittedAt)
            .ToListAsync();
        return View(documents);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditDocument(int id)
    {
        var userId = _userManager.GetUserId(User);
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id && d.SubmittedByUserId == userId);
        if (doc == null) return NotFound();
        if (doc.Status != "Pending" && doc.Status != "Rejected")
        {
            TempData["Error"] = "You cannot edit an approved document.";
            return RedirectToAction(nameof(MyDocuments));
        }

        var vm = new SubmitDocumentViewModel
        {
            Id = doc.Id,
            Title = doc.Title,
            Description = doc.Description,
            DocumentUrl = doc.DocumentUrl,
            ThumbnailUrl = doc.ThumbnailUrl,
            Era = doc.Era,
            Year = doc.Year,
            Source = doc.Source,
            DocumentType = doc.DocumentType,
            RejectionReason = doc.RejectionReason
        };
        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditDocument(int id, SubmitDocumentViewModel vm, IFormFile? DocumentFile, IFormFile? ThumbnailFile)
    {
        if (!ModelState.IsValid) return View(vm);

        var userId = _userManager.GetUserId(User);
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id && d.SubmittedByUserId == userId);
        if (doc == null) return NotFound();

        if (DocumentFile != null && DocumentFile.Length > 0)
        {
            doc.DocumentUrl = await SaveUploadedFile(DocumentFile, "documents");
        }
        else if (!string.IsNullOrEmpty(vm.DocumentUrl))
        {
            doc.DocumentUrl = vm.DocumentUrl;
        }

        if (ThumbnailFile != null && ThumbnailFile.Length > 0)
        {
            doc.ThumbnailUrl = await SaveUploadedFile(ThumbnailFile, "documents/thumbnails");
        }
        else if (!string.IsNullOrEmpty(vm.ThumbnailUrl))
        {
            doc.ThumbnailUrl = vm.ThumbnailUrl;
        }

        doc.Title = vm.Title;
        doc.Description = vm.Description;
        doc.Era = vm.Era;
        doc.Year = vm.Year;
        doc.Source = vm.Source;
        doc.DocumentType = vm.DocumentType;
        doc.Status = "Pending";
        doc.RejectionReason = null;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Your document has been updated and submitted for review.";
        return RedirectToAction(nameof(MyDocuments));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteDocument(int id)
    {
        var userId = _userManager.GetUserId(User);
        var doc = await _db.Documents.FirstOrDefaultAsync(d => d.Id == id && d.SubmittedByUserId == userId);
        if (doc != null)
        {
            _db.Documents.Remove(doc);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Your document has been deleted.";
        }
        return RedirectToAction(nameof(MyDocuments));
    }

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
}