// Controllers/AccountController.cs
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICartService _cart;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ICartService cart)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _cart = cart;
    }

    // ── KAYIT ──
    [HttpGet] public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            FirstName   = model.FirstName,
            LastName    = model.LastName,
            Email       = model.Email,
            UserName    = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Oturum sepetini kullanıcıya aktar
            var sessionId = HttpContext.Session.GetString("CartSessionId");
            if (!string.IsNullOrEmpty(sessionId))
                await _cart.MergeSessionCartAsync(user.Id, sessionId);

            TempData["Success"] = $"Hoş geldin, {user.FirstName}! Hesabın oluşturuldu.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // ── GİRİŞ ──
    [HttpGet] public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var sessionId = HttpContext.Session.GetString("CartSessionId");
                if (!string.IsNullOrEmpty(sessionId))
                    await _cart.MergeSessionCartAsync(user.Id, sessionId);
            }

            TempData["Success"] = "Başarıyla giriş yaptınız.";
            return LocalRedirect(returnUrl ?? "/");
        }

        if (result.IsLockedOut)
            ModelState.AddModelError(string.Empty, "Hesabınız geçici olarak kilitlendi. Lütfen daha sonra tekrar deneyin.");
        else
            ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");

        return View(model);
    }

    // ── ÇIKIŞ ──
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ── HESABIM ──
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
        return View(user);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string firstName, string lastName, string phoneNumber)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        user.FirstName   = firstName;
        user.LastName    = lastName;
        user.PhoneNumber = phoneNumber;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = "Profil bilgilerin güncellendi.";
        return RedirectToAction("Profile");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            TempData["Error"] = "Yeni şifreler eşleşmiyor.";
            return RedirectToAction("Profile");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login");

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (result.Succeeded)
            TempData["Success"] = "Şifren başarıyla değiştirildi.";
        else
            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));

        return RedirectToAction("Profile");
    }
}
