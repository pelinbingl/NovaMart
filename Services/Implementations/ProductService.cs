// Services/Implementations/ProductService.cs
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementations;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    public ProductService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Product>> GetFeaturedAsync(int count = 8) =>
        await _db.Products
            .Where(p => p.IsActive && p.IsFeatured)
            .Include(p => p.Category)
            .OrderByDescending(p => p.SalesCount)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, string? sortBy = null, int page = 1, int pageSize = 12)
    {
        var q = _db.Products
            .Where(p => p.IsActive && p.CategoryId == categoryId)
            .Include(p => p.Category)
            .AsQueryable();

        q = sortBy switch
        {
            "price_asc"  => q.OrderBy(p => p.Price),
            "price_desc" => q.OrderByDescending(p => p.Price),
            "newest"     => q.OrderByDescending(p => p.CreatedAt),
            "rating"     => q.OrderByDescending(p => p.AverageRating),
            "bestseller" => q.OrderByDescending(p => p.SalesCount),
            _            => q.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.SalesCount)
        };

        return await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id) =>
        await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .Include(p => p.Reviews.Where(r => r.IsApproved).OrderByDescending(r => r.CreatedAt).Take(10))
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<Product?> GetBySlugAsync(string slug) =>
        await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

    public async Task<int> GetCountByCategoryAsync(int categoryId) =>
        await _db.Products.CountAsync(p => p.IsActive && p.CategoryId == categoryId);

    public async Task<Product> CreateAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        product.Slug = GenerateSlug(product.Name);
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(int id)
    {
        var p = await _db.Products.FindAsync(id);
        if (p != null) { p.IsActive = false; await _db.SaveChangesAsync(); }
    }

    public async Task<IEnumerable<Product>> SearchAsync(string query) =>
        await _db.Products
            .Where(p => p.IsActive && (
                p.Name.ToLower().Contains(query.ToLower()) ||
                (p.Brand != null && p.Brand.ToLower().Contains(query.ToLower())) ||
                p.Category.Name.ToLower().Contains(query.ToLower())))
            .Include(p => p.Category)
            .Take(20)
            .ToListAsync();

    private static string GenerateSlug(string name) =>
        System.Text.RegularExpressions.Regex
            .Replace(name.ToLowerInvariant().Replace(" ", "-"), @"[^a-z0-9\-]", "")
            + "-" + Random.Shared.Next(1000, 9999);
}
