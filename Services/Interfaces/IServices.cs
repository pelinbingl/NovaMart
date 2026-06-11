// Services/Interfaces/IProductService.cs
using ECommerce.Models;

namespace ECommerce.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<Product>> GetFeaturedAsync(int count = 8);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, string? sortBy = null, int page = 1, int pageSize = 12);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<int> GetCountByCategoryAsync(int categoryId);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<IEnumerable<Product>> SearchAsync(string query);
}

// Services/Interfaces/ICategoryService.cs
public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllActiveAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(int id);
}

// Services/Interfaces/ICartService.cs
public interface ICartService
{
    Task<List<CartItem>> GetCartItemsAsync(string? userId, string? sessionId);
    Task AddToCartAsync(string? userId, string? sessionId, int productId, int quantity, string? variants = null);
    Task UpdateQuantityAsync(int cartItemId, int quantity);
    Task RemoveFromCartAsync(int cartItemId);
    Task ClearCartAsync(string? userId, string? sessionId);
    Task MergeSessionCartAsync(string userId, string sessionId);
    Task<decimal> GetCartTotalAsync(string? userId, string? sessionId);
}

// Services/Interfaces/IOrderService.cs
public interface IOrderService
{
    Task<Order> CreateOrderAsync(string userId, int addressId, string? couponCode, string paymentMethod);
    Task<Order?> GetByIdAsync(int id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
    Task<IEnumerable<Order>> GetAllAsync(int page = 1, int pageSize = 20);
    Task UpdateStatusAsync(int orderId, OrderStatus status);
    Task UpdatePaymentStatusAsync(int orderId, PaymentStatus status, string? paymentId = null);
}

// Services/Interfaces/IPaymentService.cs
public interface IPaymentService
{
    Task<PaymentResult> InitiatePaymentAsync(Order order, string callbackUrl);
    Task<PaymentResult> VerifyCallbackAsync(string token);
}

public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? PaymentId { get; set; }
    public string? RedirectUrl { get; set; }
    public string? ErrorMessage { get; set; }
    public string? HtmlContent { get; set; }
}
