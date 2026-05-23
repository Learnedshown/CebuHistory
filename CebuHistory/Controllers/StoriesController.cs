using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;

namespace CebuHistory.Controllers;

public class StoriesController : Controller
{
    private readonly ApplicationDbContext _db;
    public StoriesController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? era, string? category, string? q)
    {
        var query = _db.Stories.Where(s => s.Status == "Published").AsQueryable();

        if (!string.IsNullOrEmpty(era)) query = query.Where(s => s.Era == era);
        if (!string.IsNullOrEmpty(category)) query = query.Where(s => s.Category == category);
        if (!string.IsNullOrEmpty(q))
            query = query.Where(s => s.Title.Contains(q) || s.Excerpt.Contains(q));

        var all = await _db.Stories.Where(s => s.Status == "Published").ToListAsync();

        var vm = new StoriesViewModel
        {
            Stories = await query.OrderByDescending(s => s.PublishedAt).ToListAsync(),
            Eras = all.Select(s => s.Era).Distinct().OrderBy(e => e).ToList(),
            Categories = all.Select(s => s.Category).Distinct().OrderBy(c => c).ToList(),
            ActiveEra = era,
            ActiveCategory = category,
            SearchQuery = q,
        };
        return View(vm);
    }

    public async Task<IActionResult> Detail(string slug)
    {
        var story = await _db.Stories.FirstOrDefaultAsync(s => s.Slug == slug && s.Status == "Published");
        if (story == null) return NotFound();

        // Increment views
        story.Views++;
        await _db.SaveChangesAsync();

        ViewBag.Related = await _db.Stories
            .Where(s => s.Id != story.Id && s.Status == "Published" && (s.Era == story.Era || s.Category == story.Category))
            .Take(3).ToListAsync();

        return View(story);
    }
}