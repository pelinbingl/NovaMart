// Models/CartItem.cs
namespace ECommerce.Models;

public class CartItem
{
    public int Id { get; set; }
    public string? UserId { get; set; }           // null = session cart
    public string? SessionId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? SelectedVariants { get; set; }  // JSON
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public ApplicationUser? User { get; set; }
}

// Models/Review.cs
public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string? Title { get; set; }
    public string? Body { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Product Product { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

// Models/Address.cs
public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty; // "Ev", "İş"
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public ApplicationUser User { get; set; } = null!;
}

// Models/WishlistItem.cs
public class WishlistItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

// Models/Coupon.cs
public class Coupon
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal? MinOrderAmount { get; set; }
    public int? MaxUsageCount { get; set; }
    public int UsedCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
}
