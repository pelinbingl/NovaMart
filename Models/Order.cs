// Models/Order.cs
namespace ECommerce.Models;

public enum OrderStatus
{
    Pending = 0,
    PaymentPending = 1,
    Paid = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7
}

public enum PaymentStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Refunded = 3
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? IyzicoPaymentId { get; set; }
    public string? IyzicoToken { get; set; }

    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }

    public string? CouponCode { get; set; }
    public string? Notes { get; set; }

    // Shipping address snapshot
    public string ShippingFullName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddressLine { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingDistrict { get; set; } = string.Empty;
    public string ShippingZip { get; set; } = string.Empty;

    public string? TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Nav
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    public static string GenerateOrderNumber() =>
        $"NM{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductEmoji { get; set; }
    public string? SelectedVariants { get; set; } // JSON: {"Color":"Black","Storage":"512GB"}
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
