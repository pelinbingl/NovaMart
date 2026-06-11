// Areas/Admin/Controllers/DashboardController.cs
using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var now   = DateTime.UtcNow;
        var month = new DateTime(now.Year, now.Month, 1);

        ViewBag.TotalRevenue      = await _db.Orders.Where(o => o.PaymentStatus == PaymentStatus.Success).SumAsync(o => (decimal?)o.Total) ?? 0;
        ViewBag.MonthRevenue      = await _db.Orders.Where(o => o.PaymentStatus == PaymentStatus.Success && o.CreatedAt >= month).SumAsync(o => (decimal?)o.Total) ?? 0;
        ViewBag.TotalOrders       = await _db.Orders.CountAsync();
        ViewBag.PendingOrders     = await _db.Orders.CountAsync(o => o.Status == OrderStatus.PaymentPending || o.Status == OrderStatus.Processing);
        ViewBag.TotalProducts     = await _db.Products.CountAsync(p => p.IsActive);
        ViewBag.LowStockProducts  = await _db.Products.CountAsync(p => p.IsActive && p.StockQuantity < 10);
        ViewBag.TotalCustomers    = await _db.Users.CountAsync();
        ViewBag.NewCustomers      = await _db.Users.CountAsync(u => u.CreatedAt >= month);

        ViewBag.RecentOrders = await _db.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Take(8)
            .ToListAsync();

        ViewBag.TopProducts = await _db.Products
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.SalesCount)
            .Take(5)
            .ToListAsync();

        return View();
    }
}
