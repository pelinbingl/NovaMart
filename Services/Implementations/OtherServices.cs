// Services/Implementations/CategoryService.cs
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;
    public CategoryService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Category>> GetAllActiveAsync() =>
        await _db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

    public async Task<Category?> GetByIdAsync(int id) =>
        await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category?> GetBySlugAsync(string slug) =>
        await _db.Categories.FirstOrDefaultAsync(c => c.Slug == slug && c.IsActive);

    public async Task<Category> CreateAsync(Category category)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _db.Categories.Update(category);
        await _db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var c = await _db.Categories.FindAsync(id);
        if (c != null) { c.IsActive = false; await _db.SaveChangesAsync(); }
    }
}

// ─────────────────────────────────────────────────────────
// Services/Implementations/CartService.cs
public class CartService : ICartService
{
    private readonly AppDbContext _db;
    public CartService(AppDbContext db) => _db = db;

    private IQueryable<CartItem> Query(string? userId, string? sessionId) =>
        _db.CartItems
           .Include(c => c.Product)
           .Where(c => userId != null ? c.UserId == userId : c.SessionId == sessionId);

    public async Task<List<CartItem>> GetCartItemsAsync(string? userId, string? sessionId) =>
        await Query(userId, sessionId).ToListAsync();

    public async Task AddToCartAsync(string? userId, string? sessionId, int productId, int quantity, string? variants = null)
    {
        var existing = await _db.CartItems.FirstOrDefaultAsync(c =>
            (userId != null ? c.UserId == userId : c.SessionId == sessionId) &&
            c.ProductId == productId &&
            c.SelectedVariants == variants);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            _db.CartItems.Add(new CartItem
            {
                UserId = userId,
                SessionId = sessionId,
                ProductId = productId,
                Quantity = quantity,
                SelectedVariants = variants
            });
        }
        await _db.SaveChangesAsync();
    }

    public async Task UpdateQuantityAsync(int cartItemId, int quantity)
    {
        var item = await _db.CartItems.FindAsync(cartItemId);
        if (item == null) return;
        if (quantity <= 0) _db.CartItems.Remove(item);
        else item.Quantity = quantity;
        await _db.SaveChangesAsync();
    }

    public async Task RemoveFromCartAsync(int cartItemId)
    {
        var item = await _db.CartItems.FindAsync(cartItemId);
        if (item != null) { _db.CartItems.Remove(item); await _db.SaveChangesAsync(); }
    }

    public async Task ClearCartAsync(string? userId, string? sessionId)
    {
        var items = await Query(userId, sessionId).ToListAsync();
        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

    public async Task MergeSessionCartAsync(string userId, string sessionId)
    {
        var sessionItems = await _db.CartItems.Where(c => c.SessionId == sessionId).ToListAsync();
        foreach (var item in sessionItems)
        {
            var existing = await _db.CartItems.FirstOrDefaultAsync(c =>
                c.UserId == userId && c.ProductId == item.ProductId && c.SelectedVariants == item.SelectedVariants);
            if (existing != null) existing.Quantity += item.Quantity;
            else { item.UserId = userId; item.SessionId = null; }
        }
        await _db.SaveChangesAsync();
    }

    public async Task<decimal> GetCartTotalAsync(string? userId, string? sessionId)
    {
        var items = await Query(userId, sessionId).ToListAsync();
        return items.Sum(c => c.Product.Price * c.Quantity);
    }
}

// ─────────────────────────────────────────────────────────
// Services/Implementations/OrderService.cs
public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly ICartService _cart;
    public OrderService(AppDbContext db, ICartService cart) { _db = db; _cart = cart; }

    public async Task<Order> CreateOrderAsync(string userId, int addressId, string? couponCode, string paymentMethod)
    {
        var cartItems = await _cart.GetCartItemsAsync(userId, null);
        if (!cartItems.Any()) throw new InvalidOperationException("Sepet boş.");

        var address = await _db.Addresses.FindAsync(addressId)
            ?? throw new InvalidOperationException("Adres bulunamadı.");

        // Coupon
        decimal discountAmt = 0;
        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var coupon = await _db.Coupons.FirstOrDefaultAsync(c =>
                c.Code == couponCode.ToUpper() && c.IsActive &&
                (c.ExpiresAt == null || c.ExpiresAt > DateTime.UtcNow) &&
                (c.MaxUsageCount == null || c.UsedCount < c.MaxUsageCount));
            if (coupon != null)
            {
                var subtotalForCoupon = cartItems.Sum(i => i.Product.Price * i.Quantity);
                if (coupon.MinOrderAmount == null || subtotalForCoupon >= coupon.MinOrderAmount)
                {
                    discountAmt = subtotalForCoupon * (coupon.DiscountPercent / 100);
                    if (coupon.MaxDiscountAmount.HasValue && discountAmt > coupon.MaxDiscountAmount)
                        discountAmt = coupon.MaxDiscountAmount.Value;
                    coupon.UsedCount++;
                }
            }
        }

        var subtotal = cartItems.Sum(i => i.Product.Price * i.Quantity);
        var shipping = subtotal >= 500 ? 0 : 39.90m;
        var tax = (subtotal - discountAmt) * 0.18m;

        var order = new Order
        {
            OrderNumber = Order.GenerateOrderNumber(),
            UserId = userId,
            Status = OrderStatus.PaymentPending,
            PaymentMethod = paymentMethod,
            Subtotal = subtotal,
            DiscountAmount = discountAmt,
            ShippingCost = shipping,
            TaxAmount = tax,
            Total = subtotal - discountAmt + shipping + tax,
            CouponCode = couponCode,
            ShippingFullName = address.FullName,
            ShippingPhone = address.Phone,
            ShippingAddressLine = address.AddressLine,
            ShippingCity = address.City,
            ShippingDistrict = address.District,
            ShippingZip = address.ZipCode,
            Items = cartItems.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductEmoji = i.Product.EmojiIcon,
                SelectedVariants = i.SelectedVariants,
                Quantity = i.Quantity,
                UnitPrice = i.Product.Price,
                TotalPrice = i.Product.Price * i.Quantity
            }).ToList()
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        // Update stock
        foreach (var item in cartItems)
        {
            var product = await _db.Products.FindAsync(item.ProductId);
            if (product != null)
            {
                product.StockQuantity = Math.Max(0, product.StockQuantity - item.Quantity);
                product.SalesCount += item.Quantity;
            }
        }
        await _db.SaveChangesAsync();
        await _cart.ClearCartAsync(userId, null);
        return order;
    }

    public async Task<Order?> GetByIdAsync(int id) =>
        await _db.Orders.Include(o => o.Items).Include(o => o.User).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber) =>
        await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId) =>
        await _db.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetAllAsync(int page = 1, int pageSize = 20) =>
        await _db.Orders
            .Include(o => o.User)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync();

    public async Task UpdateStatusAsync(int orderId, OrderStatus status)
    {
        var o = await _db.Orders.FindAsync(orderId);
        if (o != null) { o.Status = status; o.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(); }
    }

    public async Task UpdatePaymentStatusAsync(int orderId, PaymentStatus status, string? paymentId = null)
    {
        var o = await _db.Orders.FindAsync(orderId);
        if (o == null) return;
        o.PaymentStatus = status;
        if (paymentId != null) o.IyzicoPaymentId = paymentId;
        if (status == PaymentStatus.Success) o.Status = OrderStatus.Paid;
        o.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
