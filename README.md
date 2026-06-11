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

# 🛒 NovaMart — Full-Featured ASP.NET Core 8 E-Commerce Template

A modern and scalable e-commerce application built with **ASP.NET Core 8 MVC**, **Entity Framework Core 8**, **ASP.NET Identity**, and **PostgreSQL/SQL Server**.

## 🚀 Features

- ✅ ASP.NET Core 8 MVC Architecture
- ✅ Entity Framework Core 8
- ✅ PostgreSQL & SQL Server Support
- ✅ ASP.NET Core Identity Authentication
- ✅ Role-Based Authorization (Admin / Customer)
- ✅ Product & Category Management
- ✅ Shopping Cart (Guest & Authenticated Users)
- ✅ Order Management System
- ✅ Coupon & Discount Support
- ✅ Wishlist Functionality
- ✅ Customer Reviews
- ✅ Inventory Tracking
- ✅ iyzico Payment Integration
- ✅ Responsive Bootstrap 5 UI
- ✅ Seed Data Generation

---

## 📸 Screenshots

> Add screenshots of your Home Page, Product Details, Shopping Cart, Checkout, and Admin Dashboard here.

---

## 🛠 Technologies

| Technology | Version |
|------------|----------|
| ASP.NET Core MVC | 8.0 |
| Entity Framework Core | 8.0 |
| ASP.NET Identity | 8.0 |
| PostgreSQL | Latest |
| SQL Server | Optional |
| Bootstrap | 5 |
| iyzico API | Latest |

---

## 📁 Project Structure

```text
ECommerce/
├── Areas/Admin/
│   ├── Controllers/
│   ├── Models/
│   └── Views/
├── Controllers/
├── Data/
├── Models/
├── Services/
├── Views/
├── wwwroot/
├── appsettings.json
└── Program.cs
```

---

## ⚙️ Installation

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/NovaMart.git
cd NovaMart
```

### 2. Configure Database

Update the connection string in:

```json
appsettings.json
```

Example PostgreSQL connection:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Database=novamart;Username=postgres;Password=YOUR_PASSWORD"
}
```

---

### 3. Apply Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

If EF CLI is not installed:

```bash
dotnet tool install --global dotnet-ef
```

---

### 4. Run the Application

```bash
dotnet run
```

---

## 🔐 Default Admin Account

| Field | Value |
|---------|---------|
| Email | admin@novamart.com |
| Password | Admin123! |

Admin Panel:

```text
https://localhost:5001/Admin
```

---

## 💳 iyzico Sandbox Setup

1. Create a free sandbox account.
2. Copy your API Key and Secret Key.
3. Add them to `appsettings.json`.

### Test Card

| Field | Value |
|---------|---------|
| Card Number | 5528790000000008 |
| Expiry | 12/30 |
| CVV | 123 |

---

## 👥 User Roles

### Admin

- Manage Products
- Manage Categories
- Manage Orders
- Manage Users
- Access Dashboard

### Customer

- Browse Products
- Add to Cart
- Place Orders
- Manage Profile
- Track Orders

### Guest

- Browse Products
- Session-Based Cart

---

## 🌱 Seed Data

The application automatically generates:

- 6 Categories
- 8 Sample Products
- 3 Discount Coupons:
  - NOVA10
  - INDIRIM
  - YENIYIL
- 1 Administrator Account

---

## 📈 Future Improvements

- Product Variants
- Multi-Vendor Marketplace
- Email Notifications
- Invoice Generation
- Advanced Analytics Dashboard
- Product Recommendations
- Mobile Application API

---

## 🤝 Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss your ideas.

---

## 📄 License

This project is available for educational, portfolio, and commercial starter-project use.

---

## 👨‍💻 Author

**Pelin Bingöl**

ASP.NET Core Developer

GitHub: https://github.com/yourusername
LinkedIn: https://linkedin.com/in/yourprofile
