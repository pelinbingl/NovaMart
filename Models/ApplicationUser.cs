// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Nav
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    public string FullName => $"{FirstName} {LastName}";
}
