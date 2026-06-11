# NovaMart — Tam Kapsamlı ASP.NET Core 8 E-Ticaret Şablonu

## 🚀 Kurulum (3 adım)

### 1. Veritabanını ayarla
`appsettings.json` içindeki bağlantı dizesini düzenle:
```json
"DefaultConnection": "Host=localhost;Database=novamart;Username=postgres;Password=ŞIFREN"
```
SQL Server kullanmak istersen `Program.cs` içindeki ilgili satırı yorum dışına çıkar, `Npgsql` paketini kaldır.

### 2. Migration & çalıştır
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```
Migration komutu çalışmazsa: `dotnet tool install --global dotnet-ef`

### 3. İlk giriş
- **Admin panel:** https://localhost:5001/Admin  →  `admin@novamart.com` / `Admin123!`
- **Mağaza:** https://localhost:5001

---

## 📦 Özellikler

| Modül | Detay |
|---|---|
| **Veritabanı** | EF Core 8 + PostgreSQL (veya SQL Server) |
| **Kimlik** | ASP.NET Core Identity — kayıt, giriş, rol yönetimi |
| **Ödeme** | iyzico Checkout Form (taksit + 3D Secure) |
| **Admin Panel** | Dashboard, Ürün CRUD, Kategori CRUD, Sipariş yönetimi |
| **Sepet** | Session (misafir) + DB (giriş yapmış), merge desteği |
| **Kupon** | Yüzde indirim, min tutar, son kullanma tarihi |
| **Stok** | Sipariş tamamlandığında otomatik düşürme |

---

## 📁 Proje Yapısı

```
ECommerce/
├── Areas/Admin/             ← Admin paneli (Dashboard, Ürünler, Kategoriler, Siparişler)
├── Controllers/             ← Account, Cart, Category, Home, Order, Product
├── Data/
│   └── AppDbContext.cs      ← EF Core DbContext + Seed data
├── Models/
│   ├── ApplicationUser.cs   ← Identity kullanıcısı
│   ├── Product.cs           ← Ürün, görsel, varyant
│   ├── Category.cs
│   ├── Order.cs             ← Sipariş + kalem + enum'lar
│   ├── Supporting.cs        ← CartItem, Review, Address, Wishlist, Coupon
│   └── ViewModels/
│       └── AccountViewModels.cs
├── Services/
│   ├── Interfaces/          ← IProductService, ICategoryService, ICartService, IOrderService, IPaymentService
│   └── Implementations/     ← ProductService, CategoryService + CartService + OrderService, IyzicoPaymentService
├── Views/
│   ├── Account/             ← Login, Register, Profile
│   ├── Cart/                ← Sepet
│   ├── Category/            ← Listeleme + filtre
│   ├── Home/                ← Ana sayfa
│   ├── Orders/              ← Checkout, PaymentForm, Confirmation, MyOrders
│   └── Product/             ← Detay
├── wwwroot/
│   ├── css/site.css         ← Tüm stiller
│   └── js/site.js
├── appsettings.json         ← DB + iyzico ayarları
└── Program.cs               ← DI, Identity, Session, Seeding
```

---

## 💳 iyzico Sandbox Kurulumu

1. https://sandbox-merchant.iyzipay.com adresinden ücretsiz test hesabı aç
2. API Key ve Secret Key'i kopyala
3. `appsettings.json` içine yapıştır
4. Test kartı: `5528790000000008` / Son kullanma: `12/30` / CVV: `123`

---

## 🔐 Kullanıcı Rolleri

| Rol | Erişim |
|---|---|
| **Admin** | `/Admin/*` + tüm mağaza |
| **Customer** | Mağaza, sepet, sipariş, profil |
| **Misafir** | Mağaza + oturum bazlı sepet |

---

## 🗄️ Seed Verisi (otomatik)

- 6 kategori
- 8 örnek ürün
- 3 kupon kodu: `NOVA10`, `INDIRIM`, `YENIYIL`
- 1 admin kullanıcı: `admin@novamart.com` / `Admin123!`
