// Areas/Admin/Controllers/CategoriesController.cs
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryService _categories;
    public CategoriesController(ICategoryService categories) => _categories = categories;

    public async Task<IActionResult> Index()
        => View(await _categories.GetAllActiveAsync());

    [HttpGet]
    public IActionResult Create() => View(new Category());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category model)
    {
        ModelState.Remove("Products"); ModelState.Remove("SubCategories");
        if (!ModelState.IsValid) return View(model);
        if (string.IsNullOrWhiteSpace(model.Slug))
            model.Slug = model.Name.ToLowerInvariant().Replace(" ", "-");
        await _categories.CreateAsync(model);
        TempData["Success"] = "Kategori oluşturuldu.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var cat = await _categories.GetByIdAsync(id);
        if (cat == null) return NotFound();
        return View(cat);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Category model)
    {
        ModelState.Remove("Products"); ModelState.Remove("SubCategories");
        if (!ModelState.IsValid) return View(model);
        await _categories.UpdateAsync(model);
        TempData["Success"] = "Kategori güncellendi.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _categories.DeleteAsync(id);
        TempData["Success"] = "Kategori silindi.";
        return RedirectToAction("Index");
    }
}

// ─────────────────────────────────────────────────────────
// Areas/Admin/Controllers/OrdersController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders) => _orders = orders;

    public async Task<IActionResult> Index(int page = 1)
    {
        var orders = await _orders.GetAllAsync(page, 20);
        ViewBag.Page = page;
        return View(orders);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
    {
        await _orders.UpdateStatusAsync(orderId, status);
        TempData["Success"] = "Sipariş durumu güncellendi.";
        return RedirectToAction("Detail", new { id = orderId });
    }
}
