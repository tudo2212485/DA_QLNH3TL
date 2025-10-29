# 🍽️ Restaurant Management System (Hệ thống Quản lý Nhà hàng)

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.0-61DAFB.svg)](https://reactjs.org/)
[![SQLite](https://img.shields.io/badge/SQLite-3.x-003B57.svg)](https://www.sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Hệ thống quản lý nhà hàng toàn diện được xây dựng bằng **ASP.NET Core 9.0**, **React**, và **Entity Framework Core**. Dự án hỗ trợ quản lý đặt bàn, thực đơn, đơn hàng, nhân viên, khách hàng và thống kê doanh thu.

---

## 📋 Mục lục

- [✨ Tính năng chính](#-tính-năng-chính)
- [🛠️ Công nghệ sử dụng](#️-công-nghệ-sử-dụng)
- [📦 Cài đặt](#-cài-đặt)
- [🚀 Chạy ứng dụng](#-chạy-ứng-dụng)
- [👥 Tài khoản mặc định](#-tài-khoản-mặc-định)
- [📊 Cấu trúc dự án](#-cấu-trúc-dự-án)
- [🔐 Phân quyền](#-phân-quyền)
- [📖 API Documentation](#-api-documentation)
- [🧪 Testing](#-testing)
- [📝 License](#-license)

---

## ✨ Tính năng chính

### 🔐 Xác thực & Phân quyền
- ✅ Đăng nhập/Đăng xuất với BCrypt password hashing
- ✅ 3 phân quyền: **Admin**, **Nhân viên**, **Đầu bếp**
- ✅ Session-based authentication (Cookie)
- ✅ Policy-based authorization

### 📊 Dashboard & Thống kê
- ✅ Biểu đồ doanh thu theo tháng (Line Chart)
- ✅ Thống kê đơn hàng theo trạng thái (Pie Chart)
- ✅ Top 5 món ăn bán chạy (Bar Chart)
- ✅ Tổng hợp: Tổng đơn, Doanh thu, Khách hàng

### 🍽️ Quản lý Thực đơn
- ✅ CRUD món ăn đầy đủ
- ✅ Upload ảnh món ăn (JPG, PNG, max 1MB)
- ✅ Phân loại theo danh mục
- ✅ Tìm kiếm, lọc, sắp xếp

### 📋 Quản lý Đơn hàng
- ✅ Xem đơn hiện thời & lịch sử
- ✅ Thêm/Sửa/Xóa món trong đơn
- ✅ Chuyển bàn
- ✅ Thanh toán & In hóa đơn
- ✅ 2 chế độ xem: Card View & Table View

### 🪑 Quản lý Đặt bàn
- ✅ Đặt bàn theo tầng (Tầng 1, Tầng 2, Sân thượng)
- ✅ Chọn số khách, ngày giờ
- ✅ Quản lý trạng thái bàn (Available, Reserved, Occupied)

### 👥 Quản lý Nhân viên
- ✅ CRUD nhân viên
- ✅ Phân quyền theo vai trò
- ✅ Thống kê: Tổng NV, Mới, Thân thiết, VIP
- ✅ Tìm kiếm, lọc theo vai trò

### 👤 Quản lý Khách hàng
- ✅ Tự động tạo từ đơn hàng
- ✅ Thống kê: Tổng khách, Mới (30 ngày), Thân thiết (≥5 đơn), VIP (≥1M)
- ✅ Xem lịch sử đơn hàng của khách

### ⚙️ Thiết lập Hệ thống
- ✅ Cấu hình thông tin nhà hàng
- ✅ Giờ hoạt động, Thuế VAT
- ✅ Khôi phục cài đặt mặc định

### 🔌 RESTful API
- ✅ **Server-side Pagination** (page, pageSize, search, status, sort)
- ✅ Swagger UI Documentation (`/swagger`)
- ✅ JSON response format
- ✅ Error handling & validation

---

## 🛠️ Công nghệ sử dụng

### Backend
- **ASP.NET Core 9.0** - Web API & MVC Framework
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **BCrypt.Net** - Password Hashing
- **Swashbuckle (Swagger)** - API Documentation

### Frontend
- **React 18** - SPA Framework
- **Vite** - Build Tool
- **Chart.js** - Data Visualization
- **Boxicons** - Icon Library
- **Bootstrap 5** - CSS Framework

### Development Tools
- **.NET 9 SDK**
- **Node.js & npm**
- **Git**

---

## 📦 Cài đặt

### Yêu cầu hệ thống
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **Git** - [Download](https://git-scm.com/)

### Bước 1: Clone repository

```bash
git clone https://github.com/yourusername/DACN_QLNH2.git
cd DACN_QLNH2
```

### Bước 2: Restore packages

```bash
cd DACN_QLNH2/QLNHWebApp
dotnet restore
```

### Bước 3: Tạo database

```bash
# Áp dụng migrations
dotnet ef database update

# Database sẽ được tạo tại: ../data/QLNHDB.db
```

### Bước 4: Seed dữ liệu mẫu

Dữ liệu mẫu sẽ tự động được seed khi chạy ứng dụng lần đầu, bao gồm:
- 4 tài khoản nhân viên (1 Admin, 2 Nhân viên, 1 Đầu bếp)
- 20+ món ăn
- 19 bàn (3 tầng)
- 15 đơn hàng mẫu (dữ liệu 6 tháng)

---

## 🚀 Chạy ứng dụng

### Cách 1: Sử dụng .NET CLI (Khuyến nghị)

```bash
cd DACN_QLNH2/QLNHWebApp
dotnet run
```

### Cách 2: Sử dụng Visual Studio

1. Mở file `DACN_QLNH1.sln`
2. Chọn project `QLNHWebApp`
3. Nhấn `F5` hoặc click **Run**

### Cách 3: Sử dụng script (Windows)

```bash
cd DACN_QLNH2
.\run.bat
```

### Truy cập ứng dụng

- **Trang chủ khách hàng**: http://localhost:5000
- **Trang Admin**: http://localhost:5000/Auth/Login
- **Swagger API Docs**: http://localhost:5000/swagger

---

## 👥 Tài khoản mặc định

| Username | Password | Vai trò | Quyền |
|----------|----------|---------|-------|
| `admin` | `admin123` | Admin | Toàn quyền |
| `nhanvien` | `nv123` | Nhân viên | Quản lý đơn, đặt bàn, thực đơn |
| `daubep` | `db123` | Đầu bếp | Chỉ xem thực đơn |
| `dotrungb` | `dotrungb123` | Nhân viên | Quản lý đơn, đặt bàn, thực đơn |

---

## 📊 Cấu trúc dự án

```
DACN_QLNH2/
├── QLNHWebApp/                      # Main ASP.NET Core project
│   ├── Controllers/                 # MVC & API Controllers
│   │   ├── AdminController.cs       # Admin dashboard
│   │   ├── AuthController.cs        # Login/Logout
│   │   ├── OrderManagementController.cs
│   │   ├── AdminMenuController.cs
│   │   ├── AdminCustomerController.cs
│   │   ├── SettingsController.cs
│   │   └── Api/                     # RESTful API Controllers
│   │       ├── OrdersApiController.cs   # Pagination API
│   │       ├── MenuApiController.cs
│   │       ├── TableApiController.cs
│   │       └── ContactApiController.cs
│   ├── Models/                      # Data Models
│   │   ├── RestaurantModels.cs      # All entity models
│   │   └── RestaurantDbContext.cs   # EF Core DbContext
│   ├── Views/                       # Razor Views
│   │   ├── Admin/                   # Admin panel views
│   │   ├── Auth/                    # Login views
│   │   ├── OrderManagement/         # Order management
│   │   ├── AdminMenu/               # Menu management
│   │   ├── AdminCustomer/           # Customer management
│   │   └── Settings/                # System settings
│   ├── Services/                    # Business logic
│   │   └── DataSeederService.cs     # Seed initial data
│   ├── Migrations/                  # EF Core migrations
│   ├── wwwroot/                     # Static files
│   │   ├── images/                  # Uploaded images
│   │   └── css/                     # Stylesheets
│   ├── Program.cs                   # App configuration
│   └── appsettings.json             # App settings
├── data/                            # Database folder
│   └── QLNHDB.db                    # SQLite database
├── README.md                        # This file
└── run.bat                          # Windows run script
```

---

## 🔐 Phân quyền

### Admin (Toàn quyền)
- ✅ Dashboard với biểu đồ
- ✅ Quản lý đơn hàng
- ✅ Quản lý đặt bàn
- ✅ Quản lý thực đơn (CRUD)
- ✅ Quản lý nhân viên (CRUD)
- ✅ Quản lý khách hàng
- ✅ Thiết lập hệ thống

### Nhân viên (Quản lý vận hành)
- ✅ Quản lý đơn hàng
- ✅ Quản lý đặt bàn
- ✅ Quản lý thực đơn (CRUD)
- ❌ Không thể quản lý nhân viên
- ❌ Không thể thiết lập hệ thống

### Đầu bếp (Chỉ xem)
- ✅ Xem thực đơn
- ❌ Không có quyền khác

---

## 📖 API Documentation

### Swagger UI
Truy cập: http://localhost:5000/swagger

### Các endpoint chính

#### Orders API (Có Pagination)
```http
GET /api/Orders?page=1&pageSize=10&search=&status=&sortBy=date&sortOrder=desc
```

**Response:**
```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5,
  "hasPrevious": false,
  "hasNext": true
}
```

#### Statistics API
```http
GET /api/Orders/statistics
```

**Response:**
```json
{
  "totalOrders": 50,
  "totalRevenue": 15000000,
  "pendingOrders": 5,
  "completedOrders": 45,
  "averageOrderValue": 333333
}
```

---

## 🧪 Testing

### Test Pagination API

```bash
# Test với curl
curl "http://localhost:5000/api/Orders?page=1&pageSize=5"

# Test với browser
http://localhost:5000/swagger
→ GET /api/Orders → Try it out → Execute
```

### Test Upload ảnh

1. Login: http://localhost:5000/Auth/Login
2. Username: `admin`, Password: `admin123`
3. Vào "Thực đơn" → "Thêm món mới"
4. Chọn ảnh (JPG/PNG, <1MB)
5. Điền thông tin → Save

---

## 🤝 Contributing

Contributions, issues và feature requests đều được chào đón!

1. Fork dự án
2. Tạo branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

---

## 📝 License

Dự án này được phát hành dưới giấy phép **MIT License**.

---

## 👨‍💻 Tác giả

- **Tên nhóm**: [Nhóm 3TL]
- **Email**: [your-email@example.com]
- **GitHub**: [@yourusername](https://github.com/yourusername)

---

## 📞 Liên hệ & Hỗ trợ

Nếu bạn có bất kỳ câu hỏi nào, vui lòng:
- Mở issue trên GitHub
- Gửi email: [your-email@example.com]

---

## 🙏 Lời cảm ơn

- ASP.NET Core Team
- React Community
- Chart.js Team
- Boxicons
- Tất cả contributors

---

**⭐ Nếu dự án này hữu ích, đừng quên cho một ngôi sao! ⭐**
