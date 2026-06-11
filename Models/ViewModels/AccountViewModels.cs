// Models/ViewModels/AccountViewModels.cs
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad zorunludur.")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad zorunludur.")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon zorunludur.")]
    [Phone(ErrorMessage = "Geçerli bir telefon girin.")]
    [Display(Name = "Telefon")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Şifre Tekrar")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta girin.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Beni hatırla")]
    public bool RememberMe { get; set; }
}

public class CheckoutViewModel
{
    public List<CartItem> CartItems { get; set; } = new();
    public List<Address> Addresses { get; set; } = new();
    public int SelectedAddressId { get; set; }
    public string? CouponCode { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Shipping { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }

    // New address form
    public string? NewAddressTitle { get; set; }
    public string? NewFullName { get; set; }
    public string? NewPhone { get; set; }
    public string? NewAddressLine { get; set; }
    public string? NewDistrict { get; set; }
    public string? NewCity { get; set; }
    public string? NewZip { get; set; }
}
