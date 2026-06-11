// Data/AppDbContext.cs
using ECommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<WishlistItem> WishlistItems => Set<WishlistItem>();
    public DbSet<Coupon> Coupons => Set<Coupon>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ---------- Category ----------
        builder.Entity<Category>(e =>
        {
            e.HasIndex(c => c.Slug).IsUnique();
            e.HasOne(c => c.ParentCategory)
             .WithMany(c => c.SubCategories)
             .HasForeignKey(c => c.ParentCategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Product ----------
        builder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.Slug).IsUnique();
            e.Property(p => p.Price).HasColumnType("decimal(18,2)");
            e.Property(p => p.OriginalPrice).HasColumnType("decimal(18,2)");
            e.HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Order ----------
        builder.Entity<Order>(e =>
        {
            e.HasIndex(o => o.OrderNumber).IsUnique();
            e.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
            e.Property(o => o.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.ShippingCost).HasColumnType("decimal(18,2)");
            e.Property(o => o.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.Total).HasColumnType("decimal(18,2)");
            e.HasOne(o => o.User)
             .WithMany(u => u.Orders)
             .HasForeignKey(o => o.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- OrderItem ----------
        builder.Entity<OrderItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(i => i.TotalPrice).HasColumnType("decimal(18,2)");
            e.HasOne(i => i.Product)
             .WithMany(p => p.OrderItems)
             .HasForeignKey(i => i.ProductId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- CartItem ----------
        builder.Entity<CartItem>(e =>
        {
            e.HasOne(c => c.Product)
             .WithMany(p => p.CartItems)
             .HasForeignKey(c => c.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- WishlistItem ----------
        builder.Entity<WishlistItem>(e =>
        {
            e.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();
        });

        // ---------- Coupon ----------
        builder.Entity<Coupon>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.Property(c => c.DiscountPercent).HasColumnType("decimal(5,2)");
            e.Property(c => c.MaxDiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(c => c.MinOrderAmount).HasColumnType("decimal(18,2)");
        });

        // ---------- Seed Data ----------
        SeedData(builder);
    }

    private static void SeedData(ModelBuilder builder)
    {
        // Categories
        builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Elektronik",   Slug = "elektronik",   IconEmoji = "📱", SortOrder = 1, IsActive = true },
            new Category { Id = 2, Name = "Giyim",        Slug = "giyim",        IconEmoji = "👕", SortOrder = 2, IsActive = true },
            new Category { Id = 3, Name = "Ev & Yaşam",   Slug = "ev-yasam",     IconEmoji = "🏠", SortOrder = 3, IsActive = true },
            new Category { Id = 4, Name = "Kozmetik",     Slug = "kozmetik",     IconEmoji = "💄", SortOrder = 4, IsActive = true },
            new Category { Id = 5, Name = "Oyun & Hobi",  Slug = "oyun-hobi",    IconEmoji = "🎮", SortOrder = 5, IsActive = true },
            new Category { Id = 6, Name = "Spor",         Slug = "spor",         IconEmoji = "⚽", SortOrder = 6, IsActive = true }
        );

        // Sample products
        builder.Entity<Product>().HasData(
            new Product { Id=1,  CategoryId=1, Name="Samsung Galaxy S24 Ultra", Slug="samsung-galaxy-s24-ultra",   Price=45999, OriginalPrice=52999, StockQuantity=47, Brand="Samsung",  EmojiIcon="📱", IsActive=true, IsFeatured=true, AverageRating=4.8f, ReviewCount=2147, SalesCount=1230 },
            new Product { Id=2,  CategoryId=1, Name="iPhone 15 Pro Max 256GB",  Slug="iphone-15-pro-max-256gb",    Price=62999, OriginalPrice=null,  StockQuantity=32, Brand="Apple",    EmojiIcon="🍎", IsActive=true, IsFeatured=true, IsNew=true, AverageRating=4.9f, ReviewCount=3712 },
            new Product { Id=3,  CategoryId=1, Name="Sony WH-1000XM5",          Slug="sony-wh-1000xm5",           Price=8499,  OriginalPrice=10999, StockQuantity=88, Brand="Sony",     EmojiIcon="🎧", IsActive=true, IsFeatured=true, AverageRating=4.7f, ReviewCount=1423 },
            new Product { Id=4,  CategoryId=1, Name="MacBook Air M3 13\"",       Slug="macbook-air-m3-13",         Price=62999, OriginalPrice=null,  StockQuantity=21, Brand="Apple",    EmojiIcon="💻", IsActive=true, IsFeatured=true, IsNew=true, AverageRating=4.9f, ReviewCount=3201 },
            new Product { Id=5,  CategoryId=1, Name="Apple Watch Series 9",     Slug="apple-watch-series-9",      Price=18999, OriginalPrice=null,  StockQuantity=56, Brand="Apple",    EmojiIcon="⌚", IsActive=true, IsFeatured=true, AverageRating=4.8f, ReviewCount=975  },
            new Product { Id=6,  CategoryId=5, Name="PlayStation 5 Slim",       Slug="playstation-5-slim",        Price=22499, OriginalPrice=24999, StockQuantity=15, Brand="Sony",     EmojiIcon="🎮", IsActive=true, IsFeatured=true, AverageRating=4.9f, ReviewCount=5712 },
            new Product { Id=7,  CategoryId=2, Name="Nike Air Max 270 React",   Slug="nike-air-max-270-react",    Price=3299,  OriginalPrice=null,  StockQuantity=120,Brand="Nike",     EmojiIcon="👟", IsActive=true, IsFeatured=true, IsNew=true, AverageRating=4.9f, ReviewCount=856  },
            new Product { Id=8,  CategoryId=1, Name="Canon EOS R50 Kit",        Slug="canon-eos-r50-kit",         Price=28999, OriginalPrice=null,  StockQuantity=8,  Brand="Canon",    EmojiIcon="📸", IsActive=true, IsFeatured=false,IsNew=true, AverageRating=4.7f, ReviewCount=312  }
        );

        // Sample coupons
        builder.Entity<Coupon>().HasData(
            new Coupon { Id=1, Code="NOVA10",   DiscountPercent=10, MinOrderAmount=500,  IsActive=true },
            new Coupon { Id=2, Code="INDIRIM",  DiscountPercent=15, MinOrderAmount=1000, IsActive=true },
            new Coupon { Id=3, Code="YENIYIL",  DiscountPercent=20, MinOrderAmount=2000, MaxDiscountAmount=500, ExpiresAt=new DateTime(2025,12,31), IsActive=true }
        );
    }
}
