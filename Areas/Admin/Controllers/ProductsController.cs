// Areas/Admin/Controllers/ProductsController.cs
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IProductService _products;
    private readonly ICategoryService _categories;

    public ProductsController(AppDbContext db, IProductService products, ICategoryService categories)
    {
        _db = db; _products = products; _categories = categories;
    }

    public async Task<IActionResult> Index(string? search, int? categoryId, int page = 1)
    {
        var query = _db.Products.Include(p => p.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) || (p.Brand != null && p.Brand.Contains(search)));
        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        const int pageSize = 15;
        ViewBag.TotalCount  = await query.CountAsync();
        ViewBag.Page        = page;
        ViewBag.PageSize    = pageSize;
        ViewBag.Search      = search;
        ViewBag.CategoryId  = categoryId;
        ViewBag.Categories  = await _categories.GetAllActiveAsync();

        var products = await query.OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _categories.GetAllActiveAsync();
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
    {
        ModelState.Remove("Category");
        ModelState.Remove("Slug");
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categories.GetAllActiveAsync();
            return View(product);
        }

        if (imageFile != null && imageFile.Length > 0)
            product.MainImageUrl = await SaveImageAsync(imageFile);

        await _products.CreateAsync(product);
        TempData["Success"] = $"'{product.Name}' ürünü oluşturuldu.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return NotFound();
        ViewBag.Categories = await _categories.GetAllActiveAsync();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product, IFormFile? imageFile)
    {
        ModelState.Remove("Category");
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _categories.GetAllActiveAsync();
            return View(product);
        }

        if (imageFile != null && imageFile.Length > 0)
            product.MainImageUrl = await SaveImageAsync(imageFile);

        await _products.UpdateAsync(product);
        TempData["Success"] = "Ürün güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _products.DeleteAsync(id);
        TempData["Success"] = "Ürün silindi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p != null) { p.IsActive = !p.IsActive; await _db.SaveChangesAsync(); }
        return Json(new { isActive = p?.IsActive });
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
        Directory.CreateDirectory(uploads);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        using var stream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/products/{fileName}";
    }
}
