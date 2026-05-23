using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;
using System.Diagnostics;

namespace CebuHistory.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    public HomeController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var vm = new HomeViewModel
        {
            FeaturedStories = await _db.Stories
                .Where(s => s.IsFeatured && s.Status == "Published")
                .OrderByDescending(s => s.PublishedAt).Take(3).ToListAsync(),

            LatestStories = await _db.Stories
                .Where(s => s.Status == "Published")
                .OrderByDescending(s => s.PublishedAt).Take(4).ToListAsync(),

            RecentPhotos = await _db.Photos.OrderByDescending(p => p.UploadedAt).Take(4).ToListAsync(),

            RecentDocuments = await _db.Documents.OrderByDescending(d => d.UploadedAt).Take(4).ToListAsync(),

            TimelineEvents = await _db.TimelineEvents.OrderBy(e => e.Year).ToListAsync(),
        };
        return View(vm);
    }

    public IActionResult About() => View();
    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}