// Services/Implementations/IyzicoPaymentService.cs
using ECommerce.Services.Interfaces;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.Extensions.Configuration;
using IyziAddress = Iyzipay.Model.Address;
using AppOrder   = ECommerce.Models.Order;

namespace ECommerce.Services.Implementations;

public class IyzicoPaymentService : IPaymentService
{
    private readonly Options _options;

    public IyzicoPaymentService(IConfiguration config)
    {
        _options = new Options
        {
            ApiKey    = config["Iyzico:ApiKey"]    ?? "sandbox-api-key",
            SecretKey = config["Iyzico:SecretKey"] ?? "sandbox-secret-key",
            BaseUrl   = config["Iyzico:BaseUrl"]   ?? "https://sandbox-api.iyzipay.com"
        };
    }

    public async Task<PaymentResult> InitiatePaymentAsync(AppOrder order, string callbackUrl)
    {
        var request = new CreateCheckoutFormInitializeRequest
        {
            Locale          = Locale.TR.ToString(),
            ConversationId  = order.OrderNumber,
            Price           = order.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            PaidPrice       = order.Total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
            Currency        = Currency.TRY.ToString(),
            BasketId        = order.OrderNumber,
            PaymentGroup    = PaymentGroup.PRODUCT.ToString(),
            CallbackUrl     = callbackUrl,
            EnabledInstallments = new List<int> { 1, 2, 3, 6, 9, 12 },

            Buyer = new Buyer
            {
                Id                  = order.UserId,
                Name                = order.ShippingFullName.Split(' ').FirstOrDefault() ?? "Ad",
                Surname             = order.ShippingFullName.Split(' ').LastOrDefault() ?? "Soyad",
                GsmNumber           = order.ShippingPhone,
                Email               = order.User?.Email ?? "musteri@novamart.com",
                IdentityNumber      = "11111111110",
                RegistrationAddress = order.ShippingAddressLine,
                City                = order.ShippingCity,
                Country             = "Turkey",
                ZipCode             = order.ShippingZip
            },

            ShippingAddress = new IyziAddress
            {
                ContactName = order.ShippingFullName,
                City        = order.ShippingCity,
                Country     = "Turkey",
                Description = order.ShippingAddressLine,
                ZipCode     = order.ShippingZip
            },

            BillingAddress = new IyziAddress
            {
                ContactName = order.ShippingFullName,
                City        = order.ShippingCity,
                Country     = "Turkey",
                Description = order.ShippingAddressLine,
                ZipCode     = order.ShippingZip
            },

            BasketItems = order.Items.Select(item => new BasketItem
            {
                Id         = item.ProductId.ToString(),
                Name       = item.ProductName,
                Category1  = "Genel",
                ItemType   = BasketItemType.PHYSICAL.ToString(),
                Price      = item.TotalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
            }).ToList()
        };

        var result = await Task.Run(() =>
            CheckoutFormInitialize.Create(request, _options));

        if (result.Status == "success")
        {
            return new PaymentResult
            {
                IsSuccess   = true,
                HtmlContent = result.CheckoutFormContent,
                PaymentId   = result.Token
            };
        }

        return new PaymentResult
        {
            IsSuccess    = false,
            ErrorMessage = result.ErrorMessage ?? "Ödeme başlatılamadı."
        };
    }

    public async Task<PaymentResult> VerifyCallbackAsync(string token)
    {
        var request = new RetrieveCheckoutFormRequest
        {
            Locale         = Locale.TR.ToString(),
            Token          = token
        };

        var result = await Task.Run(() =>
            CheckoutForm.Retrieve(request, _options));

        if (result.Status == "success" && result.PaymentStatus == "SUCCESS")
        {
            return new PaymentResult
            {
                IsSuccess = true,
                PaymentId = result.PaymentId
            };
        }

        return new PaymentResult
        {
            IsSuccess    = false,
            ErrorMessage = result.ErrorMessage ?? "Ödeme doğrulanamadı."
        };
    }
}
