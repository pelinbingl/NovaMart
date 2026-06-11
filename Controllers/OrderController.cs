// Controllers/OrderController.cs
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orders;
    private readonly ICartService _cart;
    private readonly IPaymentService _payment;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;

    public OrderController(IOrderService orders, ICartService cart, IPaymentService payment,
        UserManager<ApplicationUser> userManager, AppDbContext db)
    {
        _orders = orders; _cart = cart; _payment = payment;
        _userManager = userManager; _db = db;
    }

    // ── CHECKOUT ──
    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var cartItems = await _cart.GetCartItemsAsync(user.Id, null);
        if (!cartItems.Any())
        {
            TempData["Error"] = "Sepetiniz boş.";
            return RedirectToAction("Index", "Cart");
        }

        var addresses = await _db.Addresses.Where(a => a.UserId == user.Id).ToListAsync();
        var subtotal  = cartItems.Sum(i => i.Product.Price * i.Quantity);

        var model = new CheckoutViewModel
        {
            CartItems  = cartItems,
            Addresses  = addresses,
            Subtotal   = subtotal,
            Shipping   = subtotal >= 500 ? 0 : 39.90m,
            Tax        = subtotal * 0.18m,
            Total      = subtotal + (subtotal >= 500 ? 0 : 39.90m) + subtotal * 0.18m
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        // Yeni adres kaydet
        if (model.SelectedAddressId == 0 && !string.IsNullOrWhiteSpace(model.NewAddressLine))
        {
            var newAddr = new Address
            {
                UserId      = user.Id,
                Title       = model.NewAddressTitle ?? "Adresim",
                FullName    = model.NewFullName ?? user.FullName,
                Phone       = model.NewPhone ?? user.PhoneNumber ?? "",
                AddressLine = model.NewAddressLine,
                District    = model.NewDistrict ?? "",
                City        = model.NewCity ?? "",
                ZipCode     = model.NewZip ?? "",
                IsDefault   = !(await _db.Addresses.AnyAsync(a => a.UserId == user.Id))
            };
            _db.Addresses.Add(newAddr);
            await _db.SaveChangesAsync();
            model.SelectedAddressId = newAddr.Id;
        }

        try
        {
            var order = await _orders.CreateOrderAsync(
                user.Id, model.SelectedAddressId, model.CouponCode, "iyzico");

            // iyzico ödeme formu başlat
            var callbackUrl = Url.Action("PaymentCallback", "Order", null, Request.Scheme)!;
            var payResult   = await _payment.InitiatePaymentAsync(order, callbackUrl);

            if (!payResult.IsSuccess)
            {
                TempData["Error"] = payResult.ErrorMessage;
                return RedirectToAction("Checkout");
            }

            // iyzico HTML formunu göster
            TempData["IyzicoHtml"]   = payResult.HtmlContent;
            TempData["OrderNumber"]  = order.OrderNumber;
            return RedirectToAction("PaymentForm");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Checkout");
        }
    }

    // ── ÖDEME FORMU ──
    public IActionResult PaymentForm()
    {
        ViewData["IyzicoHtml"] = TempData["IyzicoHtml"]?.ToString();
        return View();
    }

    // ── iyzico CALLBACK ──
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> PaymentCallback(string token, string status)
    {
        var verifyResult = await _payment.VerifyCallbackAsync(token);
        var order = await _orders.GetByOrderNumberAsync(
            (await _db.Orders.FirstOrDefaultAsync(o => o.IyzicoToken == token))?.OrderNumber ?? "");

        if (order == null) return RedirectToAction("Index", "Home");

        if (verifyResult.IsSuccess)
        {
            await _orders.UpdatePaymentStatusAsync(order.Id, PaymentStatus.Success, verifyResult.PaymentId);
            return RedirectToAction("Confirmation", new { orderNumber = order.OrderNumber });
        }
        else
        {
            await _orders.UpdatePaymentStatusAsync(order.Id, PaymentStatus.Failed);
            TempData["Error"] = "Ödeme başarısız: " + verifyResult.ErrorMessage;
            return RedirectToAction("Failed", new { orderNumber = order.OrderNumber });
        }
    }

    // ── ONAY SAYFASI ──
    public async Task<IActionResult> Confirmation(string orderNumber)
    {
        var order = await _orders.GetByOrderNumberAsync(orderNumber);
        if (order == null) return RedirectToAction("Index", "Home");
        return View(order);
    }

    // ── BAŞARISIZ ──
    public async Task<IActionResult> Failed(string orderNumber)
    {
        var order = await _orders.GetByOrderNumberAsync(orderNumber);
        return View(order);
    }

    // ── SİPARİŞLERİM ──
    public async Task<IActionResult> MyOrders()
    {
        var user = await _userManager.GetUserAsync(User);
        var orders = await _orders.GetUserOrdersAsync(user!.Id);
        return View(orders);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var user  = await _userManager.GetUserAsync(User);
        var order = await _orders.GetByIdAsync(id);
        if (order == null || order.UserId != user!.Id) return NotFound();
        return View(order);
    }
}
