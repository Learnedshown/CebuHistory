using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;

namespace CebuHistory.Controllers;

[Authorize]
public class SubmitPhotoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SubmitPhotoController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _db = db;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new SubmitPhotoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SubmitPhotoViewModel vm, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
        {
            TempData["Errors"] = string.Join("|", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(vm);
        }

        var userId = _userManager.GetUserId(User);
        var userName = User.Identity?.Name ?? "Anonymous";

        var imageUrl = vm.ImageUrl;
        if (ImageFile != null && ImageFile.Length > 0)
        {
            imageUrl = await SaveUploadedFile(ImageFile, "photos");
        }

        var photo = new Photo
        {
            Title = vm.Title,
            Description = vm.Description,
            ImageUrl = imageUrl ?? string.Empty,
            Era = vm.Era,
            Year = vm.Year,
            Source = vm.Source,
            Location = vm.Location,
            Photographer = vm.Photographer,
            IsPublicDomain = vm.IsPublicDomain,
            Status = "Pending",
            SubmittedByUserId = userId,
            SubmittedByName = userName,
            SubmittedAt = DateTime.UtcNow,
            UploadedAt = DateTime.UtcNow,
        };

        _db.Photos.Add(photo);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thank you! Your photo has been submitted for review.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> MyPhotos()
    {
        var userId = _userManager.GetUserId(User);
        var photos = await _db.Photos
            .Where(p => p.SubmittedByUserId == userId)
            .OrderByDescending(p => p.SubmittedAt)
            .ToListAsync();
        return View(photos);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditPhoto(int id)
    {
        var userId = _userManager.GetUserId(User);
        var photo = await _db.Photos.FirstOrDefaultAsync(p => p.Id == id && p.SubmittedByUserId == userId);
        if (photo == null) return NotFound();
        if (photo.Status != "Pending" && photo.Status != "Rejected")
        {
            TempData["Error"] = "You cannot edit an approved photo.";
            return RedirectToAction(nameof(MyPhotos));
        }

        var vm = new SubmitPhotoViewModel
        {
            Id = photo.Id,
            Title = photo.Title,
            Description = photo.Description,
            ImageUrl = photo.ImageUrl,
            Era = photo.Era,
            Year = photo.Year,
            Source = photo.Source,
            Location = photo.Location,
            Photographer = photo.Photographer,
            IsPublicDomain = photo.IsPublicDomain,
            RejectionReason = photo.RejectionReason
        };
        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPhoto(int id, SubmitPhotoViewModel vm, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid) return View(vm);

        var userId = _userManager.GetUserId(User);
        var photo = await _db.Photos.FirstOrDefaultAsync(p => p.Id == id && p.SubmittedByUserId == userId);
        if (photo == null) return NotFound();

        if (ImageFile != null && ImageFile.Length > 0)
        {
            photo.ImageUrl = await SaveUploadedFile(ImageFile, "photos");
        }
        else if (!string.IsNullOrEmpty(vm.ImageUrl))
        {
            photo.ImageUrl = vm.ImageUrl;
        }

        photo.Title = vm.Title;
        photo.Description = vm.Description;
        photo.Era = vm.Era;
        photo.Year = vm.Year;
        photo.Source = vm.Source;
        photo.Location = vm.Location;
        photo.Photographer = vm.Photographer;
        photo.IsPublicDomain = vm.IsPublicDomain;
        photo.Status = "Pending";
        photo.RejectionReason = null;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Your photo has been updated and submitted for review.";
        return RedirectToAction(nameof(MyPhotos));
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeletePhoto(int id)
    {
        var userId = _userManager.GetUserId(User);
        var photo = await _db.Photos.FirstOrDefaultAsync(p => p.Id == id && p.SubmittedByUserId == userId);
        if (photo != null)
        {
            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Your photo has been deleted.";
        }
        return RedirectToAction(nameof(MyPhotos));
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