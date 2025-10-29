# 🍽️ Restaurant Management System (Hệ Thống Quản Lý Nhà Hàng)

Hệ thống quản lý nhà hàng toàn diện được xây dựng bằng **ASP.NET Core 9.0** và **React + TypeScript**.

## 📋 Tính Năng

### 👥 Dành cho Khách Hàng
- 🏠 Trang chủ giới thiệu nhà hàng
- 📖 Xem thực đơn với hình ảnh và giá
- 🛒 Giỏ hàng đặt món
- 📅 Đặt bàn online
- 💳 Thanh toán đơn hàng
- 📧 Liên hệ với nhà hàng

### 👨‍💼 Dành cho Quản Lý
- 🔐 Đăng nhập admin
- 📊 Dashboard thống kê
- 🍴 Quản lý thực đơn (CRUD)
- 📝 Quản lý đơn đặt bàn
- 🍽️ Quản lý đơn hiện thời (đang phục vụ)
- 📜 Lịch sử hóa đơn
- 🪑 Quản lý bàn ăn
- 👥 Quản lý nhân viên
- ⚙️ Cài đặt hệ thống

## 🛠️ Công Nghệ Sử Dụng

### Backend
- **ASP.NET Core 9.0** - Web framework
- **Entity Framework Core** - ORM
- **SQLite** - Database
- **Cookie Authentication** - Xác thực
- **Swagger** - API documentation

### Frontend
- **React 18** - UI framework
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Tailwind CSS** - Styling
- **Shadcn/ui** - UI components
- **React Router** - Routing
- **Lucide Icons** - Icons

## 📁 Cấu Trúc Dự Án

```
DACN_QLNH2/
├── DACN_QLNH2/
│   ├── Controllers/           # API & MVC Controllers
│   ├── Models/               # Data models
│   ├── Views/                # Razor views (Admin)
│   ├── Services/             # Business logic
│   ├── Migrations/           # EF Core migrations
│   ├── wwwroot/              # Static files
│   └── Restaurant Management Web App/  # React frontend
│       ├── src/
│       │   ├── components/   # React components
│       │   ├── services/     # API services
│       │   └── types/        # TypeScript types
│       └── build/            # Production build
├── data/                     # SQLite database
└── scripts/                  # Helper scripts
```

## 🚀 Cài Đặt & Chạy

### Yêu Cầu
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)

### Bước 1: Clone Repository
```bash
git clone https://github.com/tudo2212485/DA_QLNH3TL.git
cd DA_QLNH3TL
```

### Bước 2: Chạy Migration (Tạo Database)
```bash
cd DACN_QLNH2/QLNHWebApp
dotnet ef database update
```

### Bước 3: Chạy Ứng Dụng

#### Cách 1: Sử dụng file run.bat (Windows)
```bash
cd ../..
run.bat
```

#### Cách 2: Chạy thủ công
```bash
cd DACN_QLNH2/QLNHWebApp
dotnet run
```

### Bước 4: Truy Cập

- **Frontend (Khách hàng)**: http://localhost:5000
- **Admin**: http://localhost:5000/Auth/Login
  - Username: `admin`
  - Password: `admin123`
- **API Swagger**: http://localhost:5000/swagger

## 📚 Dữ Liệu Mẫu

Khi chạy lần đầu, hệ thống sẽ tự động tạo:
- ✅ 1 tài khoản admin
- ✅ 10+ món ăn mẫu
- ✅ 19 bàn ăn (3 tầng)
- ✅ Thông tin nhà hàng

## 🗂️ Database Schema

### Bảng Chính
- **Employees** - Nhân viên
- **MenuItems** - Món ăn
- **Tables** - Bàn ăn
- **TableBookings** - Đơn đặt bàn
- **Orders** - Đơn hàng
- **OrderItems** - Chi tiết đơn hàng
- **RestaurantSettings** - Cài đặt

## 📱 Screenshots

*(Thêm screenshots của ứng dụng tại đây)*

## 🔧 Development

### Build Frontend
```bash
cd DACN_QLNH2/QLNHWebApp/Restaurant\ Management\ Web\ App
npm install
npm run build
```

### Create Migration
```bash
cd DACN_QLNH2/QLNHWebApp
dotnet ef migrations add YourMigrationName
dotnet ef database update
```

### Clean Build
```bash
dotnet clean
dotnet build
```

## 📝 API Endpoints

### Public API
- `GET /api/menu` - Lấy danh sách món ăn
- `GET /api/tables` - Lấy danh sách bàn
- `POST /api/booking` - Đặt bàn
- `POST /api/order` - Tạo đơn hàng

### Admin API (Yêu cầu đăng nhập)
- `GET /Admin/Dashboard` - Dashboard
- `GET /AdminMenu` - Quản lý thực đơn
- `GET /AdminBooking` - Quản lý đặt bàn
- `GET /OrderManagement` - Quản lý đơn hàng

## 🐛 Troubleshooting

### Port đã được sử dụng
```bash
# Dừng tất cả process dotnet
taskkill /F /IM dotnet.exe
```

### Database bị lỗi
```bash
# Xóa database và tạo lại
cd DACN_QLNH2/QLNHWebApp
rm QLNHDB.db
dotnet ef database update
```

### Frontend không hiển thị
```bash
# Build lại frontend và copy vào wwwroot
cd Restaurant\ Management\ Web\ App
npm run build
cp -r build/* ../wwwroot/
```

## 👨‍💻 Tác Giả

- GitHub: [@tudo2212485](https://github.com/tudo2212485)

## 📄 License

Dự án này được phát triển cho mục đích học tập.

## 🙏 Acknowledgments

- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [React](https://reactjs.org/)
- [Shadcn/ui](https://ui.shadcn.com/)
- [Tailwind CSS](https://tailwindcss.com/)

---

⭐ Nếu thấy project hữu ích, hãy cho một star nhé!

