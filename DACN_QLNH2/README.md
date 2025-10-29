# 🍽️ Hệ thống Quản lý Nhà Hàng (Restaurant Management System)

## 📋 Giới thiệu

Hệ thống quản lý nhà hàng toàn diện với giao diện admin hiện đại và trang đặt bàn trực tuyến cho khách hàng. Được xây dựng với **ASP.NET Core 9** (Backend) và **React + TypeScript** (Frontend).

---

## 🏗️ Cấu trúc dự án

```
DACN_QLNH2/
├── QLNHWebApp/                          # 🔧 Backend - ASP.NET Core MVC
│   ├── Controllers/
│   │   ├── Api/                         # RESTful API Controllers
│   │   │   ├── TableApiController.cs    # API bàn ăn & đặt bàn
│   │   │   ├── MenuApiController.cs     # API thực đơn
│   │   │   ├── OrderApiController.cs    # API đơn hàng
│   │   │   └── ContactApiController.cs  # API liên hệ
│   │   ├── Admin*.cs                    # Controllers cho Admin
│   │   ├── Auth*.cs                     # Xác thực & phân quyền
│   │   └── *.cs                         # Controllers MVC khác
│   ├── Models/
│   │   ├── DTOs/                        # 🆕 Data Transfer Objects
│   │   │   ├── BookingDTOs.cs           # Request/Response đặt bàn
│   │   │   ├── OrderDTOs.cs             # Request/Response đơn hàng
│   │   │   └── PaymentDTOs.cs           # Request/Response thanh toán
│   │   ├── RestaurantModels.cs          # Entity Models
│   │   └── RestaurantDbContext.cs       # EF Core DbContext
│   ├── Views/                           # Razor Views (Server-side rendering)
│   ├── Services/                        # Business Logic Layer
│   │   └── DataSeederService.cs         # Seed dữ liệu mẫu
│   ├── Migrations/                      # EF Core Migrations
│   ├── wwwroot/                         # Static files
│   │   ├── assets/                      # React build output
│   │   ├── images/                      # Hình ảnh món ăn
│   │   └── css/                         # Custom CSS
│   ├── Program.cs                       # Entry point
│   ├── appsettings.json                 # Configuration
│   └── QLNHDB.db                        # SQLite Database
│
├── Restaurant Management Web App/       # ⚛️ Frontend - React + TypeScript
│   ├── src/
│   │   ├── components/                  # React Components
│   │   │   ├── BookingPage.tsx          # Trang đặt bàn
│   │   │   ├── MenuPage.tsx             # Trang thực đơn
│   │   │   └── ...
│   │   ├── services/                    # API Services
│   │   │   └── api.ts                   # Axios configuration
│   │   ├── types/                       # TypeScript types
│   │   └── App.tsx                      # Main App component
│   ├── build/                           # Production build (sau khi npm run build)
│   ├── package.json
│   └── vite.config.ts                   # Vite configuration
│
├── data/                                # 💾 Database files
│   └── QLNHDB.db                        # SQLite database (shared)
│
├── IMPROVEMENT_GUIDE.md                 # 📖 Hướng dẫn cải tiến
└── README.md                            # 📄 File này
```

---

## ✨ Tính năng chính

### 🎯 **Cho Khách hàng:**
- ✅ Xem thực đơn trực tuyến với hình ảnh
- ✅ Đặt bàn trực tuyến (chọn tầng, bàn, giờ)
- ✅ Chọn món ăn trước khi đến nhà hàng
- ✅ Thanh toán trực tuyến qua VNPAY (tùy chọn)

### 👨‍💼 **Cho Admin:**
- ✅ Quản lý đặt bàn (xác nhận/từ chối)
- ✅ Quản lý thực đơn (CRUD món ăn)
- ✅ Quản lý bàn ăn (thêm/sửa/xóa)
- ✅ Đặt bàn trực tiếp (walk-in booking)
- ✅ Quản lý đơn hiện thời
- ✅ Xem lịch sử hóa đơn & thống kê doanh thu
- ✅ Quản lý nhân viên
- ✅ Cài đặt hệ thống

---

## 🚀 Hướng dẫn cài đặt & chạy

### **Yêu cầu:**
- **.NET 9 SDK**: [Download tại đây](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 18+**: [Download tại đây](https://nodejs.org/)
- **Git** (tùy chọn)

---

### **Bước 1: Clone/Download dự án**

```bash
git clone <repository-url>
cd DACN_QLNH2
```

---

### **Bước 2: Chạy Backend (ASP.NET Core)**

```bash
# Di chuyển vào thư mục backend
cd QLNHWebApp

# Restore dependencies
dotnet restore

# Chạy migration (tạo database)
dotnet ef database update

# Chạy ứng dụng
dotnet run
```

**Backend sẽ chạy tại**: `http://localhost:5000`

**Tài khoản mặc định:**
- Username: `admin`
- Password: `admin123`

---

### **Bước 3: Build Frontend (React) - Nếu có thay đổi**

```bash
# Di chuyển vào thư mục frontend
cd "Restaurant Management Web App"

# Cài đặt dependencies
npm install

# Build cho production
npm run build
```

File build sẽ được copy tự động vào `QLNHWebApp/wwwroot/`

**Lưu ý:** Nếu không có thay đổi frontend, bỏ qua bước này vì file build đã có sẵn.

---

### **Bước 4: Truy cập ứng dụng**

- **🏠 Trang chủ (Client)**: http://localhost:5000
- **🔐 Admin Dashboard**: http://localhost:5000/Admin/Dashboard
- **📚 Swagger API Docs**: http://localhost:5000/swagger

---

## 🗄️ Cơ sở dữ liệu

### **SQLite Database:**
- File: `QLNHDB.db`
- ORM: Entity Framework Core
- Migrations: `Migrations/`

### **Các bảng chính:**
- `Employees` - Nhân viên
- `Tables` - Bàn ăn
- `MenuItems` - Thực đơn
- `TableBookings` - Đặt bàn
- `Orders` - Đơn hàng (sau khi xác nhận booking)
- `OrderItems` - Chi tiết món ăn trong đơn
- `RestaurantSettings` - Cài đặt hệ thống

---

## 🔧 Technologies Stack

### **Backend:**
- ASP.NET Core 9 MVC
- Entity Framework Core (SQLite)
- BCrypt.Net (Password hashing)
- Swashbuckle (Swagger/OpenAPI)

### **Frontend:**
- React 18
- TypeScript
- Vite (Build tool)
- Axios (HTTP client)
- TailwindCSS (Styling)

---

## 📖 API Documentation

Sau khi chạy backend, truy cập **Swagger UI** để xem tài liệu API đầy đủ:

```
http://localhost:5000/swagger
```

**Một số endpoint chính:**
- `POST /api/tableapi/BookTable` - Đặt bàn
- `GET /api/menuapi/GetMenuItems` - Lấy danh sách thực đơn
- `POST /OrderManagement/AddItem` - Thêm món vào đơn
- `GET /api/tableapi/GetAvailableTables` - Lấy bàn trống

---

## 🧪 Testing

### **Test Backend:**
```bash
cd QLNHWebApp
dotnet test
```

### **Test Frontend:**
```bash
cd "Restaurant Management Web App"
npm test
```

---

## 📝 Changelog & Improvements

Xem file [`IMPROVEMENT_GUIDE.md`](./IMPROVEMENT_GUIDE.md) để biết:
- ✅ Các cải tiến đã thực hiện
- 🔄 Kế hoạch nâng cấp trong tương lai
- 🐛 Bug fixes

---

## 🤝 Đóng góp

Mọi đóng góp đều được hoan nghênh! Vui lòng:
1. Fork repository
2. Tạo branch mới (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Tạo Pull Request

---

## 📄 License

Dự án này được phát triển cho mục đích học tập.

---

## 👥 Tác giả

- **Nhóm phát triển**: [Tên nhóm/SV]
- **Liên hệ**: [Email]

---

## 🎓 Hướng dẫn cho Giảng viên

### **Cách chạy nhanh nhất:**
1. Mở terminal tại `DACN_QLNH2/QLNHWebApp/`
2. Chạy `dotnet run`
3. Truy cập `http://localhost:5000`
4. Login admin với `admin / admin123`

### **Kiểm tra code quality:**
- ✅ 0 Warnings, 0 Errors
- ✅ Password hashing với BCrypt
- ✅ Global Exception Handler
- ✅ Swagger API Documentation
- ✅ Clean Architecture với DTOs

---

**🎉 Chúc bạn sử dụng hệ thống vui vẻ!**









