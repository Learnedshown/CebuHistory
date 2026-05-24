using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CebuHistory.Data;
using CebuHistory.Models;

namespace CebuHistory.Controllers;

public class DocumentsController : Controller
{
    private readonly ApplicationDbContext _db;
    public DocumentsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? type)
    {
        // Only show documents with Status = "Published"
        var query = _db.Documents.Where(d => d.Status == "Published").AsQueryable();
        if (!string.IsNullOrEmpty(type)) query = query.Where(d => d.DocumentType == type);

        var all = await _db.Documents.Where(d => d.Status == "Published").ToListAsync();
        var vm = new DocumentsViewModel
        {
            Documents = await query.OrderByDescending(d => d.Year).ToListAsync(),
            Types = all.Select(d => d.DocumentType).Distinct().OrderBy(t => t).ToList(),
            ActiveType = type,
        };
        return View(vm);
    }
}