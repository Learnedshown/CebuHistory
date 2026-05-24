using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;

namespace CebuHistory.Controllers;

public class GalleryController : Controller
{
    private readonly ApplicationDbContext _db;
    public GalleryController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? era)
    {
        // Only show photos with Status = "Published"
        var query = _db.Photos.Where(p => p.Status == "Published").AsQueryable();
        if (!string.IsNullOrEmpty(era)) query = query.Where(p => p.Era == era);

        var all = await _db.Photos.Where(p => p.Status == "Published").ToListAsync();
        var vm = new GalleryViewModel
        {
            Photos = await query.OrderByDescending(p => p.Year).ToListAsync(),
            Eras = all.Select(p => p.Era).Distinct().OrderBy(e => e).ToList(),
            ActiveEra = era,
        };
        return View(vm);
    }
}