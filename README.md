# 🍽️ Restaurant Management System (Hệ Thống Quản Lý Nhà Hàng)

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.3.1-61DAFB?style=flat&logo=react)](https://reactjs.org/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?style=flat&logo=typescript)](https://www.typescriptlang.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> Hệ thống quản lý nhà hàng toàn diện với kiến trúc 3 lớp, được xây dựng bằng **ASP.NET Core 9.0**, **React + TypeScript** và **SQLite**. Dự án hỗ trợ đầy đủ chức năng đặt bàn online, quản lý menu, thanh toán và báo cáo thống kê.

## ✨ Điểm Nổi Bật

- 🚀 **Modern Stack**: ASP.NET Core 9.0 + React 18 + TypeScript
- 🎨 **UI/UX đẹp**: Gradient design, smooth animations, responsive
- 🔐 **Bảo mật**: Cookie-based authentication với BCrypt password hashing
- 📊 **Dashboard**: Thống kê real-time với Chart.js
- 📱 **Responsive**: Tương thích mọi thiết bị (Desktop, Tablet, Mobile)
- 🐳 **Docker Ready**: Có Dockerfile và docker-compose
- 📝 **Well Documented**: Code có comment chi tiết bằng tiếng Việt

---

## 📋 Tính Năng Chi Tiết

### 👥 Module Khách Hàng (Customer)

#### 🏠 **Trang Chủ**
- Giới thiệu nhà hàng với slider hình ảnh
- Hiển thị các món đặc sắc
- Call-to-action buttons (Đặt bàn, Xem menu)

#### 📖 **Thực Đơn (Menu)**
- Xem danh sách món ăn với hình ảnh HD
- Filter theo danh mục (Khai vị, Món chính, Tráng miệng, Đồ uống)
- Search món ăn theo tên
- Xem chi tiết món (giá, mô tả, đánh giá)
- Rating & reviews

#### 🛒 **Giỏ Hàng (Cart)**
- Thêm/xóa/cập nhật số lượng món
- Tính tổng tiền tự động
- Lưu giỏ hàng trong Session
- Preview món trước khi đặt

#### 📅 **Đặt Bàn Online (Booking)**
- **Chọn tầng** dựa trên số khách:
  - 1-4 khách: Tầng 1, 2, Sân thượng
  - 5-8 khách: Tầng 1, 2
  - 9+ khách: Tầng 1
- **Chọn bàn** với hiển thị trạng thái real-time
- **Điền thông tin**: Tên, SĐT, Ngày, Giờ, Số khách, Ghi chú
- **Validation**: Kiểm tra bàn đã đặt, số khách vượt capacity

#### 💳 **Thanh Toán (Payment)**
- 3 phương thức thanh toán:
  - 💵 Tiền mặt tại nhà hàng
  - 💳 Ví điện tử (Momo, ZaloPay, VNPay)
  - 🏦 Chuyển khoản ngân hàng
- Preview đơn hàng trước khi thanh toán
- Hiển thị QR code (nếu chọn ví/ngân hàng)
- Trang Success với thông tin booking

#### 📧 **Liên Hệ (Contact)**
- Form liên hệ với validation
- Gửi tin nhắn qua email
- Hiển thị bản đồ (Google Maps)
- Hotline, Email, Địa chỉ

---

### 👨‍💼 Module Quản Trị (Admin/Staff)

#### 🔐 **Authentication**
- Đăng nhập với Cookie Authentication
- Phân quyền: Admin, Nhân viên, Đầu bếp
- Password hashing với BCrypt
- Remember me functionality
- Session timeout

#### 📊 **Dashboard**
- Thống kê tổng quan:
  - Doanh thu hôm nay/tháng/năm
  - Số đơn hàng mới
  - Số bàn trống/đang phục vụ
  - Top món bán chạy
- Biểu đồ doanh thu (Chart.js)
- Danh sách đơn hàng gần đây
- Thông báo booking mới

#### 🍴 **Quản Lý Thực Đơn (Menu Management)**
- **CRUD món ăn**:
  - Create: Thêm món mới với upload ảnh
  - Read: Xem danh sách với filter/search
  - Update: Sửa thông tin, thay ảnh
  - Delete: Xóa món (có confirm)
- **Upload ảnh**: Hỗ trợ JPG, PNG, GIF
- **Validation**: Giá > 0, tên không trống
- **Image optimization**: Tự động rename với GUID

#### 📝 **Quản Lý Đặt Bàn (Booking Management)**
- Xem danh sách booking với filter:
  - Theo trạng thái (Pending, Confirmed, Cancelled)
  - Theo ngày đặt
  - Theo tên/SĐT khách
- **Xác nhận booking** (Quan trọng):
  - Chuyển TableBooking → Order
  - Copy OrderItems từ Booking sang Order
  - Xóa Booking cũ
  - Cập nhật trạng thái bàn
- **Từ chối booking**: Xóa booking + thông báo khách
- **Chi tiết booking**: Xem thông tin đầy đủ
- **Thêm món vào booking**: Staff có thể thêm món cho khách

#### 🍽️ **Quản Lý Hóa Đơn (Order Management)**
- Danh sách đơn hàng với 2 view:
  - Card View (mặc định)
  - Table View
- Filter theo trạng thái:
  - Đang phục vụ
  - Chưa thanh toán
  - Đã thanh toán
- **Xử lý thanh toán**:
  - Click nút "Thanh toán"
  - Cập nhật status → "Đã thanh toán"
  - Record thời gian thanh toán
- **In hóa đơn**: Generate PDF với thông tin đầy đủ
- **Xem chi tiết**: Món ăn, giá, tổng tiền
- **Thống kê**: Tổng đơn, doanh thu hôm nay

#### 🪑 **Quản Lý Bàn (Table Management)**
- Xem danh sách bàn theo tầng
- Hiển thị trạng thái:
  - 🟢 Available (Trống)
  - 🔴 Occupied (Có khách)
  - 🟡 Reserved (Đã đặt)
- **Walk-in booking**: Tạo đơn cho khách đến trực tiếp
- **CRUD bàn**: Thêm/sửa/xóa/kích hoạt bàn
- **Kiểm tra capacity**: Validation số khách vs sức chứa

#### 👥 **Quản Lý Khách Hàng (Customer Management)**
- Xem danh sách khách hàng
- Lịch sử đặt bàn của khách
- Tổng chi tiêu
- Export danh sách

#### ⚙️ **Cài Đặt Hệ Thống (Settings)**
- Thông tin nhà hàng (Tên, địa chỉ, hotline)
- Giờ mở cửa
- Logo, banner
- Email template
- Cấu hình thanh toán

---

## 🛠️ Tech Stack (Công Nghệ Sử Dụng)

### **Backend**
| Technology | Version | Purpose |
|-----------|---------|---------|
| **.NET** | 9.0 | Framework chính |
| **ASP.NET Core MVC** | 9.0 | Web framework (Views cho Admin) |
| **ASP.NET Core Web API** | 9.0 | RESTful API (Frontend gọi) |
| **Entity Framework Core** | 9.0 | ORM (Object-Relational Mapping) |
| **SQLite** | 3.x | Database (nhẹ, dễ deploy) |
| **BCrypt.Net** | Latest | Password hashing |
| **Serilog** | Latest | Logging framework |
| **Swagger/OpenAPI** | Latest | API documentation |

### **Frontend**
| Technology | Version | Purpose |
|-----------|---------|---------|
| **React** | 18.3.1 | UI Library |
| **TypeScript** | 5.x | Type safety |
| **Vite** | 5.x | Build tool (nhanh hơn Webpack) |
| **Tailwind CSS** | 3.x | Utility-first CSS framework |
| **Shadcn/ui** | Latest | Component library |
| **React Router** | 6.x | Client-side routing |
| **Lucide Icons** | Latest | Icon library |
| **Chart.js** | Latest | Biểu đồ thống kê |

### **DevOps & Tools**
- **Docker** - Containerization
- **Git** - Version control
- **Visual Studio Code** - IDE
- **Postman** - API testing
- **DB Browser for SQLite** - Database management

### **Architecture Pattern**
- **3-Layer Architecture**:
  - **Presentation Layer**: Views (Razor) + React Components
  - **Business Logic Layer**: Controllers + Services
  - **Data Access Layer**: Entity Framework Core + DbContext

---

## 📁 Cấu Trúc Dự Án (Project Structure)

```
DACN_QLNH2/
│
├── QLNHWebApp/                          # Main ASP.NET Core Project
│   │
│   ├── Controllers/                     # 🎮 Controllers
│   │   ├── Api/                        # RESTful API Controllers
│   │   │   ├── ContactApiController.cs    # API liên hệ
│   │   │   ├── MenuApiController.cs       # API menu (lấy món ăn)
│   │   │   ├── OrderApiController.cs      # ⭐ API đặt bàn & checkout
│   │   │   ├── OrdersApiController.cs     # API CRUD orders
│   │   │   └── TableApiController.cs      # API quản lý bàn
│   │   │
│   │   └── MVC/                        # MVC Controllers (trả về Views)
│   │       ├── AdminBookingController.cs  # ⭐ Quản lý booking
│   │       ├── AdminController.cs         # Dashboard admin
│   │       ├── AdminCustomerController.cs # Quản lý khách hàng
│   │       ├── AdminMenuController.cs     # ⭐ Quản lý menu (CRUD)
│   │       ├── AuthController.cs          # ⭐ Login/Logout
│   │       ├── BookingController.cs       # ⭐ Đặt bàn (khách)
│   │       ├── HomeController.cs          # Trang chủ
│   │       ├── OrderManagementController.cs # ⭐ Quản lý hóa đơn
│   │       ├── PaymentController.cs       # ⭐ Thanh toán
│   │       ├── SettingsController.cs      # Cài đặt
│   │       └── TableController.cs         # Quản lý bàn
│   │
│   ├── Models/                          # 📦 Data Models
│   │   ├── RestaurantDbContext.cs      # ⭐ Database Context
│   │   ├── MenuItem.cs                 # Model món ăn
│   │   ├── Order.cs                    # Model đơn hàng
│   │   ├── OrderItem.cs                # Model chi tiết đơn
│   │   ├── TableBooking.cs             # Model đặt bàn
│   │   ├── Table.cs                    # Model bàn ăn
│   │   ├── Employee.cs                 # Model nhân viên
│   │   └── RestaurantSettings.cs       # Model cài đặt
│   │
│   ├── Views/                           # 🎨 Razor Views (Admin UI)
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml          # Layout chung (khách)
│   │   │   ├── _AdminLayout.cshtml     # Layout admin
│   │   │   └── _ChatBot.cshtml         # Widget chatbot
│   │   │
│   │   ├── Admin/                      # Views Admin
│   │   │   ├── Index.cshtml            # Dashboard
│   │   │   └── ...
│   │   │
│   │   ├── AdminBooking/               # Views Quản lý booking
│   │   │   ├── Index.cshtml            # Danh sách booking
│   │   │   ├── Details.cshtml          # Chi tiết booking
│   │   │   └── Edit.cshtml             # Sửa booking
│   │   │
│   │   ├── AdminMenu/                  # Views Quản lý menu
│   │   │   ├── Index.cshtml            # Danh sách món
│   │   │   ├── Create.cshtml           # Thêm món
│   │   │   └── Edit.cshtml             # Sửa món
│   │   │
│   │   ├── OrderManagement/            # Views Quản lý đơn hàng
│   │   │   ├── Index.cshtml            # Danh sách đơn
│   │   │   ├── Edit.cshtml             # Sửa đơn
│   │   │   └── History.cshtml          # Lịch sử
│   │   │
│   │   ├── Auth/                       # Views Authentication
│   │   │   └── Login.cshtml            # Trang login
│   │   │
│   │   ├── Booking/                    # Views Đặt bàn (khách)
│   │   │   └── Table.cshtml            # Form đặt bàn
│   │   │
│   │   ├── Payment/                    # Views Thanh toán
│   │   │   ├── Index.cshtml            # Trang thanh toán
│   │   │   └── Success.cshtml          # Trang thành công
│   │   │
│   │   └── Table/                      # Views Quản lý bàn
│   │       └── Index.cshtml            # Sơ đồ bàn
│   │
│   ├── Services/                        # 🔧 Business Logic
│   │   └── DataSeeder.cs               # Seed dữ liệu mẫu
│   │
│   ├── Helpers/                         # 🛠️ Helper Classes
│   │   └── SessionExtensions.cs        # Extension methods cho Session
│   │
│   ├── Migrations/                      # 📜 EF Core Migrations
│   │   ├── 20250922103133_InitialCreate.cs
│   │   ├── 20251020171612_AddTableManagement.cs
│   │   └── ...
│   │
│   ├── wwwroot/                         # 📂 Static Files
│   │   ├── css/                        # CSS files
│   │   │   ├── site.css                # CSS chung
│   │   │   ├── payment.css             # CSS thanh toán
│   │   │   └── admin.css               # CSS admin
│   │   │
│   │   ├── js/                         # JavaScript files
│   │   │   ├── site.js
│   │   │   └── booking-menu-manager.js
│   │   │
│   │   ├── images/                     # Hình ảnh
│   │   │   ├── menu/                   # Ảnh món ăn
│   │   │   └── logo.png
│   │   │
│   │   └── lib/                        # Libraries (Bootstrap, jQuery, etc.)
│   │
│   ├── Restaurant Management Web App/   # ⚛️ React Frontend (SPA)
│   │   ├── src/
│   │   │   ├── components/             # React components
│   │   │   │   ├── HomePage.tsx        # Trang chủ
│   │   │   │   ├── MenuPage.tsx        # Trang menu
│   │   │   │   ├── BookingPage.tsx     # Trang đặt bàn
│   │   │   │   ├── CartPage.tsx        # Giỏ hàng
│   │   │   │   ├── PaymentPage.tsx     # Thanh toán
│   │   │   │   └── ...
│   │   │   │
│   │   │   ├── services/               # API Services
│   │   │   │   └── api.ts              # Axios API calls
│   │   │   │
│   │   │   ├── types/                  # TypeScript Interfaces
│   │   │   │   └── index.ts            # Type definitions
│   │   │   │
│   │   │   ├── hooks/                  # Custom React Hooks
│   │   │   │   └── useCart.ts          # Hook quản lý giỏ hàng
│   │   │   │
│   │   │   ├── App.tsx                 # Main App component
│   │   │   └── main.tsx                # Entry point
│   │   │
│   │   ├── build/                      # Production build
│   │   ├── package.json                # NPM dependencies
│   │   ├── vite.config.ts              # Vite config
│   │   └── tsconfig.json               # TypeScript config
│   │
│   ├── Logs/                            # 📝 Application Logs
│   │   └── log-20251129.txt
│   │
│   ├── Program.cs                       # ⭐ Entry point (Startup)
│   ├── appsettings.json                # Configuration
│   ├── appsettings.Development.json    # Dev config
│   └── QLNHWebApp.csproj               # Project file
│
├── data/                                # 💾 Database
│   └── QLNHDB.db                       # SQLite database file
│
├── scripts/                             # 🚀 Helper Scripts
│   ├── run-migrations.bat              # Chạy migrations (Windows)
│   └── run-migrations.sh               # Chạy migrations (Linux/Mac)
│
├── docs/                                # 📚 Documentation
│   └── screenshots/                    # Screenshots
│
├── Dockerfile                           # 🐳 Docker configuration
├── docker-compose.yml                   # Docker Compose
├── .gitignore                           # Git ignore rules
├── README.md                            # ⭐ File này
└── DACN_QLNH1.sln                      # Visual Studio Solution
```

### 📌 **Files Quan Trọng Cần Chú Ý:**

| File | Mô tả | Vai trò |
|------|-------|---------|
| `Program.cs` | Entry point của app | Cấu hình services, middleware, routing |
| `RestaurantDbContext.cs` | Database context | Định nghĩa tables, relationships, seed data |
| `OrderApiController.cs` | API đặt bàn | **Xử lý flow booking chính** |
| `AdminBookingController.cs` | Quản lý booking | **Confirm booking → Order** |
| `BookingController.cs` | Đặt bàn (khách) | **Khách tạo booking** |
| `PaymentController.cs` | Thanh toán | Xử lý payment methods |
| `appsettings.json` | Configuration | Connection string, logging, settings |

---

## 🚀 Hướng Dẫn Cài Đặt & Chạy (Installation Guide)

### ✅ **Yêu Cầu Hệ Thống (Prerequisites)**

| Software | Version | Download Link | Ghi chú |
|----------|---------|--------------|---------|
| **.NET SDK** | 9.0+ | [Download](https://dotnet.microsoft.com/download/dotnet/9.0) | Bắt buộc |
| **Node.js** | 18+ | [Download](https://nodejs.org/) | Cho React frontend |
| **Git** | Latest | [Download](https://git-scm.com/) | Clone repository |
| **Visual Studio Code** | Latest | [Download](https://code.visualstudio.com/) | IDE (khuyên dùng) |
| **DB Browser for SQLite** | Latest | [Download](https://sqlitebrowser.org/) | Xem database (optional) |

### 📥 **Bước 1: Clone Repository**

```bash
# Clone từ GitHub
git clone https://github.com/tudo2212485/DA_QLNH3TL.git

# Di chuyển vào thư mục
cd DA_QLNH3TL/QLNHWebApp
```

### 🗄️ **Bước 2: Tạo Database (SQLite)**

```bash
# Cách 1: Sử dụng EF Core Migrations (Khuyên dùng)
dotnet ef database update

# Cách 2: Chạy script (nếu có)
cd ../scripts
./run-migrations.bat  # Windows
./run-migrations.sh   # Linux/Mac
```

**Kết quả:** File `QLNHDB.db` sẽ được tạo trong thư mục `data/`

### ⚙️ **Bước 3: Restore Dependencies**

```bash
# Restore .NET packages
cd QLNHWebApp
dotnet restore

# (Optional) Restore React dependencies nếu muốn build frontend
cd "Restaurant Management Web App"
npm install
npm run build
```

### 🎯 **Bước 4: Chạy Ứng Dụng**

#### **Cách 1: Chạy với Visual Studio Code (Khuyên dùng)**

```bash
# 1. Mở VS Code
code .

# 2. Nhấn F5 hoặc Run → Start Debugging
# Hoặc trong Terminal:
dotnet run
```

#### **Cách 2: Chạy với Command Line**

```bash
cd QLNHWebApp
dotnet run
```

#### **Cách 3: Chạy với Docker**

```bash
# Build image
docker build -t restaurant-management .

# Chạy container
docker run -p 5000:5000 restaurant-management

# Hoặc dùng Docker Compose
docker-compose up
```

### 🌐 **Bước 5: Truy Cập Ứng Dụng**

Sau khi chạy thành công, mở trình duyệt:

| URL | Mô tả | Ghi chú |
|-----|-------|---------|
| **http://localhost:5000** | Trang chủ (Khách hàng) | React SPA |
| **http://localhost:5000/Auth/Login** | Trang đăng nhập Admin | MVC View |
| **http://localhost:5000/swagger** | API Documentation | Swagger UI |

### 🔑 **Tài Khoản Mặc Định (Default Accounts)**

#### **Admin Account:**
- **Email**: `admin@gmail.com`
- **Password**: `Admin@123`
- **Role**: Administrator (Full quyền)

#### **Staff Account:**
- **Email**: `staff@gmail.com`
- **Password**: `Staff@123`
- **Role**: Staff (Quản lý đơn hàng, booking)

#### **Chef Account:**
- **Email**: `chef@gmail.com`
- **Password**: `Chef@123`
- **Role**: Chef (Xem menu, đơn hàng)

### 🎨 **Bước 6: Build Frontend (Optional)**

Nếu bạn muốn modify React frontend:

```bash
cd "Restaurant Management Web App"

# Install dependencies
npm install

# Development mode (hot reload)
npm run dev

# Production build
npm run build

# Copy build files to wwwroot
cp -r build/* ../wwwroot/
```

---

## 🗄️ Cấu Trúc Database (Database Schema)

Hệ thống sử dụng **SQLite** với **8 bảng chính**, được quản lý bởi Entity Framework Core Migrations.

### 📊 **Sơ Đồ Quan Hệ (Entity Relationship)**

```
┌──────────────┐         ┌──────────────┐         ┌──────────────┐
│  Employees   │         │  Categories  │         │    Tables    │
│──────────────│         │──────────────│         │──────────────│
│ EmployeeId ⭐│         │ CategoryId ⭐│         │ TableId ⭐   │
│ Name         │         │ Name         │         │ TableNumber  │
│ Email        │         │ Description  │         │ Capacity     │
│ Password     │         └──────┬───────┘         │ Status       │
│ Role         │                │                 └──────┬───────┘
└──────────────┘                │                        │
                                ↓                        ↓
                        ┌──────────────┐         ┌──────────────┐
                        │  MenuItems   │         │TableBookings │
                        │──────────────│         │──────────────│
                        │ MenuItemId ⭐│         │ BookingId ⭐ │
                        │ Name         │         │ CustomerName │
                        │ Description  │         │ Phone        │
                        │ Price        │         │ TableId [FK] │
                        │ CategoryId[FK│         │ BookingTime  │
                        │ ImagePath    │         │ Status       │
                        │ IsAvailable  │         └──────┬───────┘
                        └──────┬───────┘                │
                               │                        │
                               ↓                        ↓
                        ┌──────────────┐         ┌──────────────┐
                        │ OrderItems   │◄────────┤   Orders     │
                        │──────────────│         │──────────────│
                        │OrderItemId ⭐│         │ OrderId ⭐   │
                        │ OrderId [FK] │         │ TableId [FK] │
                        │ MenuItemId[FK│         │ TotalAmount  │
                        │ Quantity     │         │ Status       │
                        │ Price        │         │ OrderDate    │
                        │ Subtotal     │         │ CustomerEmail│
                        └──────────────┘         └──────────────┘
                                                        ↓
                                                 ┌──────────────┐
                                                 │SystemSettings│
                                                 │──────────────│
                                                 │ SettingId ⭐ │
                                                 │ Key          │
                                                 │ Value        │
                                                 │ Description  │
                                                 └──────────────┘
```

### 📋 **Chi Tiết Các Bảng (Table Details)**

#### 1. **`Employees` - Quản Lý Nhân Viên**
```sql
EmployeeId      INTEGER PRIMARY KEY AUTOINCREMENT
Name            TEXT NOT NULL
Email           TEXT UNIQUE NOT NULL
PasswordHash    TEXT NOT NULL  -- BCrypt hashed
Role            TEXT NOT NULL  -- Admin, Staff, Chef
CreatedAt       DATETIME DEFAULT CURRENT_TIMESTAMP
```
**Mục đích:** Lưu thông tin nhân viên và tài khoản admin

#### 2. **`Categories` - Danh Mục Món Ăn**
```sql
CategoryId      INTEGER PRIMARY KEY AUTOINCREMENT
Name            TEXT NOT NULL UNIQUE
Description     TEXT
DisplayOrder    INTEGER DEFAULT 0
```
**Mục đích:** Phân loại món ăn (Khai vị, Món chính, Tráng miệng...)

#### 3. **`MenuItems` - Thực Đơn**
```sql
MenuItemId      INTEGER PRIMARY KEY AUTOINCREMENT
Name            TEXT NOT NULL
Description     TEXT
Price           REAL NOT NULL
CategoryId      INTEGER FOREIGN KEY → Categories(CategoryId)
ImagePath       TEXT
IsAvailable     INTEGER DEFAULT 1  -- 1: Còn, 0: Hết
CreatedAt       DATETIME DEFAULT CURRENT_TIMESTAMP
```
**Mục đích:** Lưu thông tin chi tiết món ăn

#### 4. **`Tables` - Bàn Ăn**
```sql
TableId         INTEGER PRIMARY KEY AUTOINCREMENT
TableNumber     INTEGER NOT NULL UNIQUE
Capacity        INTEGER NOT NULL  -- Số người tối đa
Status          TEXT NOT NULL     -- Available, Occupied, Reserved
```
**Mục đích:** Quản lý trạng thái bàn ăn

#### 5. **`TableBookings` - Đặt Bàn**
```sql
BookingId       INTEGER PRIMARY KEY AUTOINCREMENT
CustomerName    TEXT NOT NULL
Phone           TEXT NOT NULL
Email           TEXT
TableId         INTEGER FOREIGN KEY → Tables(TableId)
BookingDate     DATETIME NOT NULL
BookingTime     TEXT NOT NULL
NumberOfGuests  INTEGER NOT NULL
Status          TEXT NOT NULL     -- Pending, Confirmed, Cancelled, Completed
SpecialRequests TEXT
CreatedAt       DATETIME DEFAULT CURRENT_TIMESTAMP
```
**Mục đích:** Xử lý yêu cầu đặt bàn trước của khách

#### 6. **`Orders` - Đơn Hàng**
```sql
OrderId         INTEGER PRIMARY KEY AUTOINCREMENT
TableId         INTEGER FOREIGN KEY → Tables(TableId)
CustomerEmail   TEXT
TotalAmount     REAL NOT NULL
Status          TEXT NOT NULL     -- Pending, Confirmed, Preparing, Completed, Cancelled
PaymentMethod   TEXT              -- Cash, Card, VNPay
OrderDate       DATETIME DEFAULT CURRENT_TIMESTAMP
CompletedAt     DATETIME
```
**Mục đích:** Lưu thông tin đơn hàng tổng thể

#### 7. **`OrderItems` - Chi Tiết Đơn Hàng**
```sql
OrderItemId     INTEGER PRIMARY KEY AUTOINCREMENT
OrderId         INTEGER FOREIGN KEY → Orders(OrderId)
MenuItemId      INTEGER FOREIGN KEY → MenuItems(MenuItemId)
Quantity        INTEGER NOT NULL
Price           REAL NOT NULL     -- Giá tại thời điểm đặt
Subtotal        REAL NOT NULL     -- Price * Quantity
Note            TEXT              -- Yêu cầu đặc biệt (ít cay, nhiều đá...)
BookingId       INTEGER FOREIGN KEY → TableBookings(BookingId)
```
**Mục đích:** Lưu từng món trong đơn hàng

#### 8. **`SystemSettings` - Cấu Hình Hệ Thống**
```sql
SettingId       INTEGER PRIMARY KEY AUTOINCREMENT
Key             TEXT NOT NULL UNIQUE
Value           TEXT NOT NULL
Description     TEXT
LastUpdated     DATETIME DEFAULT CURRENT_TIMESTAMP
```
**Mục đích:** Lưu cấu hình như tên nhà hàng, giờ mở cửa, phí giao hàng...

### 🔗 **Quan Hệ Giữa Các Bảng (Relationships)**

```
Categories (1) ──→ (N) MenuItems
Tables (1) ──→ (N) TableBookings
Tables (1) ──→ (N) Orders
Orders (1) ──→ (N) OrderItems
MenuItems (1) ──→ (N) OrderItems
TableBookings (1) ──→ (N) OrderItems [Optional]
```

### 📌 **Seeding Data (Dữ Liệu Mẫu Ban Đầu)**

Khi chạy migration lần đầu, hệ thống tự động tạo:
- ✅ **3 Employee Accounts** (Admin, Staff, Chef)
- ✅ **5 Categories** (Khai vị, Món chính, Món phụ, Tráng miệng, Đồ uống)
- ✅ **20+ MenuItems** (Phở, Bún chả, Cơm rang, Bánh flan, Trà đá...)
- ✅ **19 Tables** (3 tầng: Tầng 1 có 7 bàn, Tầng 2 có 6 bàn, Tầng 3 có 6 bàn)
- ✅ **8 System Settings** (Tên nhà hàng, địa chỉ, hotline, email, giờ mở cửa...)

---

## 📸 Screenshots (Hình Ảnh Demo)

### 🏠 **Trang Chủ (Homepage)**
<img src="docs/screenshots/homepage.png" alt="Homepage" width="800"/>

*Giao diện React SPA với Tailwind CSS + Shadcn/ui*

---

### 🍽️ **Trang Menu (Menu Page)**
<img src="docs/screenshots/menu.png" alt="Menu Page" width="800"/>

*Hiển thị món ăn theo danh mục, filter, search*

---

### 📅 **Đặt Bàn (Booking Page)**
<img src="docs/screenshots/booking.png" alt="Booking Page" width="800"/>

*Form đặt bàn với chọn ngày, giờ, số người*

---

### 🛒 **Giỏ Hàng (Cart)**
<img src="docs/screenshots/cart.png" alt="Cart" width="800"/>

*Quản lý món đã chọn, cập nhật số lượng*

---

### 💳 **Thanh Toán (Payment)**
<img src="docs/screenshots/payment.png" alt="Payment Page" width="800"/>

*Trang thanh toán với nhiều phương thức: Tiền mặt, Thẻ, VNPay*

---

### 📊 **Admin Dashboard**
<img src="docs/screenshots/admin-dashboard.png" alt="Admin Dashboard" width="800"/>

*Thống kê doanh thu, đơn hàng, biểu đồ Chart.js*

---

### 📋 **Quản Lý Booking**
<img src="docs/screenshots/admin-booking.png" alt="Admin Booking" width="800"/>

*Danh sách booking, confirm/cancel, xem chi tiết*

---

### 🍴 **Quản Lý Menu**
<img src="docs/screenshots/admin-menu.png" alt="Admin Menu" width="800"/>

*CRUD món ăn, upload ảnh, chỉnh giá*

---

### 🧾 **Quản Lý Đơn Hàng**
<img src="docs/screenshots/admin-orders.png" alt="Admin Orders" width="800"/>

*Theo dõi trạng thái đơn, cập nhật, xem lịch sử*

---

> **Lưu ý:** Screenshots demo có thể được thêm vào thư mục `docs/screenshots/` sau khi deploy production.

---

## 🔧 Hướng Dẫn Development (Development Guide)

### 🛠️ **Setup Development Environment**

#### **1. Clone & Install Dependencies**
```bash
# Clone repository
git clone https://github.com/tudo2212485/DA_QLNH3TL.git
cd DA_QLNH3TL/QLNHWebApp

# Restore .NET packages
dotnet restore

# Install React dependencies
cd "Restaurant Management Web App"
npm install
```

#### **2. Configure Database**
```bash
# Tạo database từ migrations
dotnet ef database update

# Kiểm tra database đã tạo thành công
ls ../data/QLNHDB.db
```

---

### ⚙️ **Thao Tác Thường Dùng**

#### **Build React Frontend**
```bash
# Development mode (hot reload)
cd "Restaurant Management Web App"
npm run dev

# Production build
npm run build

# Copy build files to wwwroot
xcopy /E /I /Y build\* ..\wwwroot\
```

#### **Run Backend**
```bash
# Run trong development mode
cd QLNHWebApp
dotnet run

# Run với watch mode (auto-reload khi code thay đổi)
dotnet watch run

# Build production
dotnet publish -c Release -o ./publish
```

#### **Database Migrations**
```bash
# Tạo migration mới
dotnet ef migrations add YourMigrationName

# Xem SQL sẽ được thực thi
dotnet ef migrations script

# Apply migration vào database
dotnet ef database update

# Rollback về migration trước đó
dotnet ef database update PreviousMigrationName

# Xóa migration cuối cùng (chưa apply)
dotnet ef migrations remove
```

#### **Clean Build**
```bash
# Clean solution
dotnet clean

# Build lại
dotnet build

# Restore + Clean + Build
dotnet restore; dotnet clean; dotnet build
```

---

### 🎨 **Customize & Extend**

#### **Thêm Món Ăn Mới (Add New Menu Item)**
1. Vào Admin Panel → Menu Management
2. Click "Add New Item"
3. Upload ảnh món ăn (lưu vào `wwwroot/images/menu/`)
4. Điền thông tin: Tên, Giá, Mô tả, Danh mục

#### **Thêm Table Mới (Add New Table)**
```csharp
// Trong DataSeeder.cs hoặc Admin Panel
var newTable = new Table 
{
    TableNumber = 20,
    Capacity = 4,
    Status = "Available",
    Floor = 3
};
context.Tables.Add(newTable);
context.SaveChanges();
```

#### **Thêm Role Mới (Add New Employee Role)**
```csharp
// Trong Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy => 
        policy.RequireRole("Admin", "Manager"));
});
```

---

### 🧪 **Testing**

#### **Manual Testing Checklist**
```
✅ Customer Flow:
  - Xem trang chủ
  - Xem menu, filter, search
  - Thêm món vào giỏ hàng
  - Đặt bàn
  - Checkout & thanh toán
  - Nhận email xác nhận

✅ Admin Flow:
  - Login/Logout
  - Xem dashboard
  - CRUD Menu Items
  - Confirm/Cancel Booking
  - Cập nhật Order Status
  - Xem thống kê
```

#### **API Testing với Swagger**
```bash
# Chạy app
dotnet run

# Mở Swagger UI
# http://localhost:5000/swagger

# Test các endpoint:
# - GET /api/Menu
# - POST /api/Order/book-table
# - GET /api/Tables
```

---

### 📦 **Deployment**

#### **Deploy với Docker**
```bash
# Build Docker image
docker build -t restaurant-management:latest .

# Run container
docker run -d -p 8080:5000 --name restaurant-app restaurant-management:latest

# Kiểm tra logs
docker logs restaurant-app

# Stop & remove
docker stop restaurant-app
docker rm restaurant-app
```

#### **Deploy lên IIS (Windows)**
```bash
# Publish app
dotnet publish -c Release -o ./publish

# Copy publish folder vào IIS directory
# Cấu hình IIS:
# - Application Pool: .NET CLR Version = No Managed Code
# - Binding: Port 80 hoặc 443 (HTTPS)
```

---

### 🔍 **Debug & Troubleshooting Tips**

#### **Debug Backend**
```bash
# Xem logs trong Logs/ folder
cat Logs/log-20251129.txt

# Enable verbose logging trong appsettings.json
"Logging": {
  "LogLevel": {
    "Default": "Debug"  # Information → Debug
  }
}
```

#### **Debug Frontend**
```bash
# Mở React DevTools trong Chrome
# Xem Network tab để kiểm tra API calls
# Xem Console log để check errors
```

---

## 📡 API Documentation (API Endpoints)

Hệ thống cung cấp **RESTful API** cho frontend React và có thể tích hợp với các hệ thống khác.

### 🌐 **Base URL**
```
Development: http://localhost:5000
Production: https://your-domain.com
```

---

### 🔓 **Public API (Không cần authentication)**

#### **1. Menu API**

```http
GET /api/Menu
```
**Mô tả:** Lấy danh sách tất cả món ăn  
**Response:**
```json
[
  {
    "menuItemId": 1,
    "name": "Phở Bò",
    "description": "Phở bò Hà Nội truyền thống",
    "price": 50000,
    "categoryName": "Món chính",
    "imagePath": "/images/menu/pho-bo.jpg",
    "isAvailable": true
  }
]
```

---

```http
GET /api/Menu/categories
```
**Mô tả:** Lấy danh sách danh mục món ăn  
**Response:**
```json
[
  {
    "categoryId": 1,
    "name": "Món chính",
    "description": "Các món chính như phở, bún, cơm",
    "displayOrder": 1
  }
]
```

---

#### **2. Table API**

```http
GET /api/Table
```
**Mô tả:** Lấy danh sách tất cả bàn ăn  
**Response:**
```json
[
  {
    "tableId": 1,
    "tableNumber": 1,
    "capacity": 4,
    "status": "Available",
    "floor": 1
  }
]
```

---

```http
GET /api/Table/available?date=2024-12-01&time=19:00
```
**Mô tả:** Lấy danh sách bàn trống tại thời điểm cụ thể  
**Query Parameters:**
- `date` (string, required): Ngày đặt (YYYY-MM-DD)
- `time` (string, required): Giờ đặt (HH:mm)

**Response:**
```json
[
  {
    "tableId": 5,
    "tableNumber": 5,
    "capacity": 6,
    "status": "Available"
  }
]
```

---

#### **3. Contact API**

```http
POST /api/Contact
```
**Mô tả:** Gửi form liên hệ  
**Request Body:**
```json
{
  "name": "Nguyễn Văn A",
  "email": "nguyenvana@gmail.com",
  "phone": "0987654321",
  "message": "Tôi muốn hỏi về menu tiệc cưới"
}
```
**Response:**
```json
{
  "success": true,
  "message": "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi trong 24h."
}
```

---

#### **4. Order API (Booking & Checkout)**

```http
POST /api/Order/book-table
```
**Mô tả:** Đặt bàn trước  
**Request Body:**
```json
{
  "customerName": "Trần Thị B",
  "phone": "0912345678",
  "email": "tranthib@gmail.com",
  "tableId": 5,
  "bookingDate": "2024-12-05T19:00:00",
  "numberOfGuests": 4,
  "specialRequests": "Cần ghế em bé"
}
```
**Response:**
```json
{
  "success": true,
  "bookingId": 123,
  "message": "Đặt bàn thành công! Mã booking: #123",
  "bookingDetails": {
    "tableNumber": 5,
    "bookingTime": "2024-12-05 19:00",
    "status": "Pending"
  }
}
```

---

```http
POST /api/Order/checkout
```
**Mô tả:** Tạo đơn hàng và thanh toán  
**Request Body:**
```json
{
  "tableId": 5,
  "customerEmail": "customer@gmail.com",
  "items": [
    {
      "menuItemId": 1,
      "quantity": 2,
      "note": "Ít muối"
    },
    {
      "menuItemId": 3,
      "quantity": 1
    }
  ],
  "paymentMethod": "Cash",
  "bookingId": 123  // Optional: nếu đã đặt bàn trước
}
```
**Response:**
```json
{
  "success": true,
  "orderId": 456,
  "totalAmount": 150000,
  "message": "Đơn hàng #456 đã được tạo thành công!",
  "orderDetails": {
    "orderDate": "2024-12-05T14:30:00",
    "status": "Pending",
    "items": [
      {
        "name": "Phở Bò",
        "quantity": 2,
        "price": 50000,
        "subtotal": 100000
      }
    ]
  }
}
```

---

### 🔒 **Admin API (Yêu cầu Authentication)**

> **Lưu ý:** Các endpoint sau yêu cầu đăng nhập với role Admin hoặc Staff

#### **1. Authentication**

```http
POST /Auth/Login
```
**Mô tả:** Đăng nhập admin  
**Request Body:**
```json
{
  "email": "admin@gmail.com",
  "password": "Admin@123"
}
```
**Response:** Cookie-based session (Redirect to /Admin/Dashboard)

---

```http
POST /Auth/Logout
```
**Mô tả:** Đăng xuất  
**Response:** Redirect to /Auth/Login

---

#### **2. Dashboard API**

```http
GET /Admin/Dashboard
```
**Mô tả:** Trang dashboard tổng quan  
**Response:** HTML View (Razor Page)

---

#### **3. Menu Management API**

```http
GET /AdminMenu
```
**Mô tả:** Danh sách món ăn (Admin view)

---

```http
POST /AdminMenu/Create
```
**Mô tả:** Tạo món ăn mới  
**Request Body:** Form Data (multipart/form-data)
```
Name: Bún Chả Hà Nội
Description: Bún chả truyền thống
Price: 45000
CategoryId: 2
Image: [File Upload]
IsAvailable: true
```

---

```http
POST /AdminMenu/Edit/{id}
```
**Mô tả:** Cập nhật món ăn

---

```http
POST /AdminMenu/Delete/{id}
```
**Mô tả:** Xóa món ăn

---

#### **4. Booking Management API**

```http
GET /AdminBooking
```
**Mô tả:** Danh sách booking (Admin view)

---

```http
POST /AdminBooking/Confirm/{id}
```
**Mô tả:** Xác nhận booking (Pending → Confirmed)  
**Response:** Chuyển booking thành Order

---

```http
POST /AdminBooking/Cancel/{id}
```
**Mô tả:** Hủy booking

---

#### **5. Order Management API**

```http
GET /OrderManagement
```
**Mô tả:** Danh sách đơn hàng

---

```http
POST /OrderManagement/UpdateStatus/{id}
```
**Mô tả:** Cập nhật trạng thái đơn hàng  
**Request Body:**
```json
{
  "status": "Preparing"  // Pending → Confirmed → Preparing → Completed
}
```

---

### 🧪 **Test API với Swagger**

Truy cập Swagger UI để test API:
```
http://localhost:5000/swagger
```

Swagger cung cấp:
- ✅ Danh sách tất cả endpoints
- ✅ Request/Response schema
- ✅ Try it out (test trực tiếp)
- ✅ Authentication support

---

### 🔐 **Authentication & Authorization**

Hệ thống sử dụng **Cookie-based Authentication**:

```csharp
// Login flow
1. POST /Auth/Login → Tạo cookie session
2. Browser tự động gửi cookie trong subsequent requests
3. Backend verify cookie → Authorize user

// Roles
- Admin: Full quyền
- Staff: Quản lý booking, orders (không thể xóa món ăn)
- Chef: Chỉ xem orders (read-only)
```

---

## 🐛 Troubleshooting (Xử Lý Lỗi Thường Gặp)

### ❌ **Lỗi: Port 5000 đã được sử dụng**

**Triệu chứng:**
```
Error: Unable to bind to http://localhost:5000 on the IPv4 loopback interface: 'Address already in use'
```

**Cách sửa (Windows):**
```powershell
# Tìm process đang dùng port 5000
netstat -ano | findstr :5000

# Kill process (thay <PID> bằng số trong cột cuối)
taskkill /F /PID <PID>

# Hoặc kill tất cả dotnet processes
taskkill /F /IM dotnet.exe
```

**Cách sửa (Linux/Mac):**
```bash
# Tìm process
lsof -i :5000

# Kill process
kill -9 <PID>
```

---

### ❌ **Lỗi: Database migration failed**

**Triệu chứng:**
```
Build failed. The following build errors occurred:
Microsoft.Data.Sqlite.SqliteException: SQLite Error 1: 'table X already exists'
```

**Cách sửa:**
```bash
# Xóa database và migration history
cd QLNHWebApp
rm ../data/QLNHDB.db

# Tạo lại database
dotnet ef database update

# Kiểm tra database đã tạo thành công
ls ../data/QLNHDB.db
```

---

### ❌ **Lỗi: Frontend không hiển thị (404 Not Found)**

**Triệu chứng:**
- Trang chủ http://localhost:5000 hiển thị 404
- React app không load

**Cách sửa:**
```bash
# Build lại React app
cd "Restaurant Management Web App"
npm install
npm run build

# Copy build files vào wwwroot
xcopy /E /I /Y build\* ..\wwwroot\

# Hoặc trên Linux/Mac
cp -r build/* ../wwwroot/

# Restart server
cd ../
dotnet run
```

---

### ❌ **Lỗi: Login không hoạt động (Cookie authentication failed)**

**Triệu chứng:**
- Đăng nhập thành công nhưng bị redirect lại trang login
- Cookie không được lưu

**Cách sửa:**
```csharp
// Kiểm tra appsettings.json
{
  "Https": {
    "Enabled": false  // Disable HTTPS trong development
  }
}

// Hoặc trong Program.cs
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.None  // Development only
});
```

---

### ❌ **Lỗi: EF Core tools không tìm thấy**

**Triệu chứng:**
```
Could not execute because the specified command or file was not found.
Possible reasons for this include:
  * The command 'dotnet ef' is not installed.
```

**Cách sửa:**
```bash
# Install EF Core CLI tools globally
dotnet tool install --global dotnet-ef

# Hoặc update nếu đã cài
dotnet tool update --global dotnet-ef

# Kiểm tra version
dotnet ef --version
```

---

### ❌ **Lỗi: CORS error khi gọi API từ React**

**Triệu chứng:**
```
Access to XMLHttpRequest at 'http://localhost:5000/api/Menu' from origin 'http://localhost:3000' has been blocked by CORS policy
```

**Cách sửa:**
```csharp
// Trong Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")  // Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("AllowReactApp");
```

---

### ❌ **Lỗi: Swagger không hiển thị**

**Triệu chứng:**
- http://localhost:5000/swagger trả về 404

**Cách sửa:**
```csharp
// Trong Program.cs, đảm bảo Swagger được enable
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```

---

### ❌ **Lỗi: NPM install failed (React)**

**Triệu chứng:**
```
npm ERR! code ERESOLVE
npm ERR! ERESOLVE unable to resolve dependency tree
```

**Cách sửa:**
```bash
# Xóa node_modules và package-lock.json
rm -rf node_modules package-lock.json

# Clear npm cache
npm cache clean --force

# Install lại với legacy peer deps
npm install --legacy-peer-deps
```

---

### ⚠️ **Lỗi: "The database is locked" (SQLite)**

**Triệu chứng:**
```
Microsoft.Data.Sqlite.SqliteException: SQLite Error 5: 'database is locked'
```

**Cách sửa:**
```bash
# Đóng tất cả connections đến database
# 1. Stop server (Ctrl+C)
# 2. Đóng DB Browser for SQLite (nếu đang mở)
# 3. Restart server
dotnet run

# Nếu vẫn lỗi, copy database ra backup và tạo mới
cp ../data/QLNHDB.db ../data/QLNHDB_backup.db
rm ../data/QLNHDB.db
dotnet ef database update
```

---

### 🔍 **Debug Tips**

#### **1. Xem Application Logs**
```bash
# Logs được lưu trong Logs/ folder
cat Logs/log-20251129.txt

# Hoặc tail real-time (Linux/Mac)
tail -f Logs/log-$(date +%Y%m%d).txt
```

#### **2. Enable Verbose Logging**
```json
// appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

#### **3. Check Database Content**
```bash
# Sử dụng DB Browser for SQLite
# Download: https://sqlitebrowser.org/

# Hoặc dùng SQLite CLI
sqlite3 data/QLNHDB.db

# Xem danh sách bảng
.tables

# Xem dữ liệu
SELECT * FROM Employees;
SELECT * FROM MenuItems;
```

---

### 📞 **Cần Trợ Giúp Thêm?**

Nếu gặp lỗi không có trong danh sách trên:
1. ✅ Kiểm tra **Logs/** folder
2. ✅ Xem **Chrome DevTools Console** (F12)
3. ✅ Kiểm tra **Swagger UI** để test API
4. ✅ Tạo **GitHub Issue** với error message đầy đủ

---

## 🎯 Roadmap & Future Features

### 🚀 **Tính Năng Sẽ Phát Triển (Coming Soon)**

- [ ] **💬 Chatbot AI** - Tích hợp chatbot hỗ trợ khách hàng 24/7
- [ ] **📧 Email Notifications** - Gửi email xác nhận booking, order status
- [ ] **💳 VNPay Integration** - Thanh toán online qua VNPay
- [ ] **📱 Mobile App** - React Native app cho iOS/Android
- [ ] **🔔 Real-time Notifications** - SignalR cho thông báo real-time
- [ ] **📊 Advanced Analytics** - Dashboard với nhiều biểu đồ hơn
- [ ] **🍕 Multi-Restaurant** - Hỗ trợ nhiều chi nhánh
- [ ] **🎁 Loyalty Program** - Chương trình tích điểm khách hàng thân thiết
- [ ] **📸 QR Code Menu** - Scan QR để xem menu
- [ ] **🌐 Multi-language** - Hỗ trợ tiếng Anh, tiếng Việt

### 🐛 **Known Issues**

- [ ] Payment page cần thêm loading state cho VNPay redirect
- [ ] Admin dashboard biểu đồ cần optimize performance với dataset lớn
- [ ] Mobile responsive cần improve cho tablet size

---

## 👨‍💻 Tác Giả & Đóng Góp (Author & Contributors)

### 👤 **Developer**
- **Name:** [Your Name]
- **GitHub:** [@tudo2212485](https://github.com/tudo2212485)
- **Email:** [Your Email]
- **Project:** Đồ Án Chuyên Ngành - Quản Lý Nhà Hàng (Restaurant Management System)
- **University:** [Your University]
- **Year:** 2024-2025

### 🤝 **Contributing**

Nếu bạn muốn đóng góp vào dự án:

1. **Fork repository**
   ```bash
   git clone https://github.com/tudo2212485/DA_QLNH3TL.git
   ```

2. **Tạo branch mới**
   ```bash
   git checkout -b feature/AmazingFeature
   ```

3. **Commit changes**
   ```bash
   git commit -m "Add some AmazingFeature"
   ```

4. **Push to branch**
   ```bash
   git push origin feature/AmazingFeature
   ```

5. **Tạo Pull Request**

---

## 📄 License (Giấy Phép)

Dự án này được phát triển cho **mục đích học tập** trong khuôn khổ Đồ Án Chuyên Ngành.

**License Type:** MIT License

```
MIT License

Copyright (c) 2024 [Your Name]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

---

## 🙏 Acknowledgments (Lời Cảm Ơn)

Dự án này được phát triển với sự trợ giúp của các công nghệ và công cụ sau:

### 🛠️ **Frameworks & Libraries**
- [**ASP.NET Core**](https://docs.microsoft.com/aspnet/core) - Modern web framework by Microsoft
- [**React**](https://reactjs.org/) - UI library by Meta
- [**Entity Framework Core**](https://docs.microsoft.com/ef/core/) - ORM for .NET
- [**Vite**](https://vitejs.dev/) - Next generation frontend tooling
- [**Tailwind CSS**](https://tailwindcss.com/) - Utility-first CSS framework
- [**Shadcn/ui**](https://ui.shadcn.com/) - Beautiful component library
- [**Chart.js**](https://www.chartjs.org/) - Simple yet flexible JavaScript charting

### 📚 **Documentation & Learning Resources**
- [Microsoft Learn](https://learn.microsoft.com/)
- [React Documentation](https://react.dev/)
- [Stack Overflow](https://stackoverflow.com/)
- [GitHub Community](https://github.com/)

### 🎨 **Design & Assets**
- [Lucide Icons](https://lucide.dev/) - Beautiful & consistent icon set
- [Unsplash](https://unsplash.com/) - Free high-quality images
- [Google Fonts](https://fonts.google.com/) - Web fonts

### 🧪 **Testing & Tools**
- [Postman](https://www.postman.com/) - API testing
- [Swagger/OpenAPI](https://swagger.io/) - API documentation
- [DB Browser for SQLite](https://sqlitebrowser.org/) - Database management
- [Visual Studio Code](https://code.visualstudio.com/) - Code editor

---

## 📞 Liên Hệ & Hỗ Trợ (Contact & Support)

### 💬 **Có câu hỏi? Cần hỗ trợ?**

- **GitHub Issues:** [Report Bug / Request Feature](https://github.com/tudo2212485/DA_QLNH3TL/issues)
- **Email:** [Your Email]
- **Facebook:** [Your Facebook Profile]

### 🌟 **Yêu thích dự án?**

Nếu bạn thấy dự án này hữu ích, hãy:
- ⭐ **Star repository** trên GitHub
- 🔄 **Share** với bạn bè
- 🐛 **Report bugs** để cải thiện
- 💡 **Suggest features** mới

---

<div align="center">

### 🎓 **Đồ Án Chuyên Ngành - Hệ Thống Quản Lý Nhà Hàng**

**Developed with ❤️ by [@tudo2212485](https://github.com/tudo2212485)**

[![GitHub stars](https://img.shields.io/github/stars/tudo2212485/DA_QLNH3TL?style=social)](https://github.com/tudo2212485/DA_QLNH3TL/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/tudo2212485/DA_QLNH3TL?style=social)](https://github.com/tudo2212485/DA_QLNH3TL/network/members)
[![GitHub issues](https://img.shields.io/github/issues/tudo2212485/DA_QLNH3TL)](https://github.com/tudo2212485/DA_QLNH3TL/issues)

⭐ **Star us on GitHub — it motivates us a lot!**

</div>

