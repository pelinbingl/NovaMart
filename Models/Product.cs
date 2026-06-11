// Models/Product.cs
namespace ECommerce.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? Brand { get; set; }
    public string? SKU { get; set; }
    public string? MainImageUrl { get; set; }
    public string? EmojiIcon { get; set; } = "📦";
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; }
    public bool IsNew { get; set; }
    public float AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int SalesCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // FK
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Nav
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();

    // Computed
    public decimal DiscountPercent => OriginalPrice.HasValue && OriginalPrice > 0
        ? Math.Round((1 - Price / OriginalPrice.Value) * 100)
        : 0;
    public bool IsOnSale => OriginalPrice.HasValue && Price < OriginalPrice;
    public bool InStock => StockQuantity > 0;
}

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public Product Product { get; set; } = null!;
}

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string VariantType { get; set; } = string.Empty; // "Color", "Size", "Storage"
    public string VariantValue { get; set; } = string.Empty;
    public decimal? PriceAdjustment { get; set; }
    public int StockQuantity { get; set; }
    public Product Product { get; set; } = null!;
}
