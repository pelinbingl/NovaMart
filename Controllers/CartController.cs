// Controllers/CartController.cs
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models;

namespace ECommerce.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cart;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ICartService cart, UserManager<ApplicationUser> userManager)
    {
        _cart = cart;
        _userManager = userManager;
    }

    private string? UserId => _userManager.GetUserId(User);
    private string SessionId => HttpContext.Session.GetString("CartSessionId") ?? SetSession();

    private string SetSession()
    {
        var id = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("CartSessionId", id);
        return id;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _cart.GetCartItemsAsync(UserId, User.Identity?.IsAuthenticated == true ? null : SessionId);
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int productId, int quantity = 1, string? variants = null)
    {
        await _cart.AddToCartAsync(UserId, User.Identity?.IsAuthenticated == true ? null : SessionId, productId, quantity, variants);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQty(int cartItemId, int quantity)
    {
        await _cart.UpdateQuantityAsync(cartItemId, quantity);
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        await _cart.RemoveFromCartAsync(cartItemId);
        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> GetCount()
    {
        var items = await _cart.GetCartItemsAsync(UserId, User.Identity?.IsAuthenticated == true ? null : SessionId);
        return Json(new { count = items.Sum(i => i.Quantity) });
    }
}
