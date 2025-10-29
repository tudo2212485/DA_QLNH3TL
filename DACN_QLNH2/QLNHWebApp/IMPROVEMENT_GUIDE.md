# 🚀 HƯỚNG DẪN CẢI TIẾN DỰ ÁN - TỪ 7.5/10 → 9/10

---

## 📋 MỤC LỤC

1. [Swagger API Documentation](#1-swagger-api-documentation)
2. [README.md với Screenshots](#2-readmemd-với-screenshots)
3. [Fix Warnings](#3-fix-warnings)
4. [Password Hashing toàn bộ](#4-password-hashing-toàn-bộ)
5. [Global Exception Handler](#5-global-exception-handler)
6. [Logging System](#6-logging-system)

---

## 1. SWAGGER API DOCUMENTATION

### ✅ **Công dụng là gì?**

Swagger tạo ra một **trang web tự động** giúp:
- ✅ **Xem tất cả API** của bạn một cách trực quan
- ✅ **Test API ngay trên trình duyệt** (không cần Postman!)
- ✅ **Xem request/response** của mỗi endpoint
- ✅ **Ấn tượng với giảng viên** - trông rất chuyên nghiệp!

### 🎯 **Ví dụ thực tế:**

**TRƯỚC:**
```
Giảng viên hỏi: "Em có API đặt bàn không? Nhận tham số gì?"
Bạn: "Dạ có ạ... em phải mở code lên xem..."
→ Mất thời gian, không chuyên nghiệp!
```

**SAU:**
```
Bạn: "Dạ thầy vào http://localhost:5000/swagger ạ!"
Giảng viên thấy:
  ┌──────────────────────────────────────────┐
  │ POST /api/tableapi/BookTable             │
  │ ├─ customerName: string (required)       │
  │ ├─ phone: string (required)              │
  │ ├─ tableId: int (required)               │
  │ └─ orderItems: array (optional)          │
  │                                          │
  │ [Try it out] ← Click để test luôn!      │
  └──────────────────────────────────────────┘
→ Chuyên nghiệp, dễ demo, dễ test!
```

### 📝 **Đã làm (tự động):**

✅ Đã customize Swagger trong `Program.cs`:
- Title: "API Quản Lý Nhà Hàng"
- Version: "v1.0"
- Description chi tiết
- Contact info

### 🚀 **Cách sử dụng:**

1. Chạy app: `dotnet run`
2. Mở browser: `http://localhost:5000/swagger`
3. Xem tất cả API của bạn!
4. Click **"Try it out"** → Nhập params → **Execute** để test!

---

## 2. README.MD VỚI SCREENSHOTS

### ✅ **Công dụng là gì?**

README là **"bộ mặt"** của dự án. Giảng viên đọc README **TRƯỚC** khi chạy code!

README tốt giúp:
- ✅ Giảng viên **hiểu ngay** dự án làm gì
- ✅ **Setup nhanh** không cần hỏi
- ✅ **Ấn tượng** hơn rất nhiều (có thể +0.5 → +1 điểm!)

### 🎯 **Ví dụ thực tế:**

**README CŨ (hiện tại):**
```markdown
# QLNHWebApp
Ứng dụng quản lý nhà hàng...
```
→ **Nhàm chán, không có hình ảnh, không rõ làm gì!**

**README MỚI (nên có):**
```markdown
# 🍽️ Hệ Thống Quản Lý Nhà Hàng

![Dashboard](screenshots/dashboard.png)

## ✨ Tính năng nổi bật

### 👥 **Dành cho Khách hàng**
- 🔍 Xem thực đơn với hình ảnh
- 📅 Đặt bàn online (chọn tầng, chọn bàn)
- 🛒 Giỏ hàng (thêm/xóa/sửa món)
- 💳 Thanh toán trực tuyến

### 👨‍💼 **Dành cho Admin**
- 📊 Dashboard với thống kê real-time
- ✅ Xác nhận/Từ chối đặt bàn
- 🍽️ Quản lý đơn hàng (thêm/xóa/sửa món)
- 🏃 Walk-in booking (khách đến trực tiếp)
- 📜 Lịch sử hóa đơn với filter ngày tháng
- 🍕 CRUD Menu, Nhân viên, Khách hàng

## 📸 Screenshots

### Trang chủ Khách hàng
![Homepage](screenshots/homepage.png)

### Admin Dashboard
![Admin Dashboard](screenshots/admin-dashboard.png)

### Quản lý Đơn hàng
![Order Management](screenshots/order-management.png)

## 🛠️ Tech Stack

**Backend:**
- ASP.NET Core 9 (MVC)
- Entity Framework Core 9
- SQLite Database
- BCrypt (Password Hashing)

**Frontend:**
- React 18 + TypeScript
- Vite (Build tool)
- Bootstrap 5
- SweetAlert2

## 🚀 Cài đặt & Chạy

### Yêu cầu
- .NET 9 SDK
- Node.js 18+

### Backend
```bash
cd QLNHWebApp
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend (React)
```bash
cd "Restaurant Management Web App"
npm install
npm run build
```

### Truy cập
- Backend: http://localhost:5000
- Swagger API: http://localhost:5000/swagger

### Demo Account
- **Username:** `admin`
- **Password:** `admin123`

## 📂 Cấu trúc thư mục

```
QLNHWebApp/
├── Controllers/          # API & MVC Controllers
│   ├── Api/             # RESTful APIs
│   └── Admin*.cs        # Admin Controllers
├── Models/              # Database Models
├── Views/               # Razor Views
├── Services/            # Business Logic
├── Migrations/          # EF Core Migrations
└── wwwroot/             # Static files

Restaurant Management Web App/   # React SPA
├── src/
│   ├── components/
│   ├── services/
│   └── types/
└── build/               # Production build
```

## 🎯 Tính năng nổi bật (Chi tiết)

### 1. Đặt bàn Online
- Khách chọn số người → Hệ thống gợi ý tầng phù hợp
- Chọn bàn theo sức chứa
- Validation: Không đặt trùng, không vượt capacity
- Tự động tạo Order khi admin xác nhận

### 2. Walk-in Booking
- Admin đặt bàn cho khách đến trực tiếp
- Real-time check bàn trống
- Tạo Order ngay lập tức

### 3. Quản lý Đơn hàng
- **Đơn hiện thời:** Đang phục vụ, chưa thanh toán
- **Lịch sử hóa đơn:** Đã thanh toán, có filter ngày
- Thêm/xóa/sửa món ngay trong đơn
- Chuyển bàn
- In hóa đơn

### 4. Modal "Thêm món" đẹp
- Grid 3 cột
- Tìm kiếm real-time
- Nhập số lượng trực tiếp
- Không reload toàn trang

## 📊 Database Schema

```
Tables
├── Orders (Đơn hàng)
├── OrderItems (Chi tiết đơn)
├── TableBookings (Đặt bàn)
├── Tables (Bàn ăn)
├── MenuItems (Thực đơn)
├── Employees (Nhân viên)
└── RestaurantSettings (Cấu hình)
```

## 🔧 API Endpoints (Swagger)

Xem chi tiết tại: http://localhost:5000/swagger

**Booking:**
- `POST /api/tableapi/BookTable`
- `GET /api/tableapi/GetTablesByFloor`

**Order:**
- `GET /OrderManagement`
- `POST /OrderManagement/AddItem`
- `POST /OrderManagement/RemoveItem`

**Admin:**
- `POST /AdminBooking/Confirm`
- `POST /AdminBooking/Reject`
- `POST /AdminBooking/CreateWalkIn`

## 👨‍💻 Tác giả
- **Họ tên:** Your Name
- **Email:** your@email.com
- **GitHub:** github.com/yourname

## 📝 License
MIT License - Do whatever you want!

---

**⭐ Nếu thấy hay, hãy cho dự án một ngôi sao!**
```

→ **Chuyên nghiệp, đầy đủ, có hình ảnh, dễ hiểu!**

### 📝 **Cách làm:**

**Bước 1: Chụp screenshots**
```
Windows + Shift + S → Chụp màn hình
```

Chụp các trang:
1. Dashboard
2. Quản lý đặt bàn
3. Quản lý đơn hàng
4. Modal thêm món
5. Lịch sử hóa đơn
6. Frontend booking page

**Bước 2: Tạo folder**
```bash
mkdir screenshots
# Copy các ảnh vào đây
```

**Bước 3: Viết README**
- Copy template trên
- Thay đổi thông tin cá nhân
- Thêm đường dẫn screenshots

---

## 3. FIX WARNINGS

### ✅ **Công dụng là gì?**

**Build warnings = Điểm trừ!** 

Giảng viên sẽ nghĩ:
- ❌ Code không cẩn thận
- ❌ Không chuyên nghiệp
- ❌ Có thể có bug ẩn

**0 warnings = +0.5 điểm!**

### 🐛 **3 Warnings hiện tại:**

#### **Warning 1 & 2: Missing `await`**
```
TableController.cs(17,42): warning CS1998: This async method lacks 'await' operators...
BookingController.cs(26,42): warning CS1998: This async method lacks 'await' operators...
```

**Vấn đề:** 
Method khai báo `async` nhưng không có `await` bên trong → Lãng phí!

**✅ ĐÃ SỬA:**
- `TableController.SelectFloor`: Bỏ `async`
- `BookingController.SaveBookingInfo`: Bỏ `async`

#### **Warning 3: Null Reference**
```
Views/Table/SelectFloor.cshtml(24,48): warning CS8602: Dereference of a possibly null reference.
```

**Vấn đề:**
Code truy cập property của object mà object có thể `null`.

**Cách sửa:**
```csharp
// Trước (warning):
@Model.Count

// Sau (no warning):
@(Model?.Count ?? 0)
```

### 🚀 **Test kết quả:**

Sau khi fix, chạy:
```bash
dotnet build
```

Kết quả mong muốn:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## 4. PASSWORD HASHING TOÀN BỘ

### ✅ **Công dụng là gì?**

**Bảo mật password!**

Nếu database bị hack:
- ❌ **Không hash:** Hacker thấy ngay password → Nguy hiểm!
- ✅ **Có hash:** Hacker chỉ thấy chuỗi `$2a$11$xyz...` → An toàn!

### 🎯 **Ví dụ thực tế:**

**Trong database:**

| Username | Password (KHÔNG hash) | Password (Có hash) |
|----------|----------------------|-------------------|
| admin | `admin123` | `$2a$11$nOUIs5kJ.../XuQOPoJHCd` |
| user1 | `password` | `$2a$11$kR9vG3Zp.../OmXZWvAkPQ` |

→ Nếu hacker vào database, họ:
- Không hash: Thấy ngay `admin123` → Đăng nhập luôn!
- Có hash: Thấy `$2a$11$...` → Không biết password gốc!

### 📝 **Hiện trạng:**

Dự án đã có:
- ✅ Admin password đã hash bằng BCrypt
- ❌ Một số chỗ còn plain text

### 🚀 **Cách cải tiến:**

Kiểm tra tất cả nơi lưu password:

**File cần check:**
1. `Models/RestaurantModels.cs` - Employee class
2. `Services/DataSeederService.cs` - Seed data
3. `Controllers/AuthController.cs` - Login logic

**Đảm bảo:**
```csharp
// SỬA TẤT CẢ CHỖ TẠO USER

// ❌ KHÔNG BAO GIỜ LÀM:
var employee = new Employee {
    Password = "admin123"  // Plain text!
};

// ✅ LUÔN LUÔN LÀM:
var employee = new Employee {
    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
};
```

---

## 5. GLOBAL EXCEPTION HANDLER

### ✅ **Công dụng là gì?**

**Bắt MỌI lỗi trong app!**

Thay vì:
- ❌ App crash → Trang lỗi xấu
- ❌ Lỗi không được log → Khó debug
- ❌ User thấy stack trace → Không chuyên nghiệp

Với Global Exception Handler:
- ✅ App không crash
- ✅ Lỗi được log tự động
- ✅ User thấy trang lỗi đẹp
- ✅ Admin nhận email/notification

### 🎯 **Ví dụ thực tế:**

**TRƯỚC:**
```
User click "Đặt bàn" → Database lỗi
→ Trang trắng hiện: "SqlException: Connection timeout..."
→ User: "WTF???"
```

**SAU:**
```
User click "Đặt bàn" → Database lỗi
→ Trang đẹp: "Xin lỗi, có lỗi xảy ra. Vui lòng thử lại sau."
→ Log tự động: "[2024-10-28 14:30] SqlException at /api/booking..."
→ User: "OK, thử lại sau vậy!"
```

### 📝 **Cách làm:**

**Bước 1: Tạo Middleware**

File: `Middleware/GlobalExceptionHandlerMiddleware.cs`
```csharp
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message = "Đã xảy ra lỗi. Vui lòng thử lại sau.",
            error = ex.Message // Chỉ hiện trong Development
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

**Bước 2: Đăng ký trong Program.cs**
```csharp
// Thêm vào trước app.Run()
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
```

---

## 6. LOGGING SYSTEM

### ✅ **Công dụng là gì?**

**Ghi lại MỌI hành động quan trọng!**

Khi có bug:
- ❌ Không log: "Tại sao lỗi nhỉ??? Không biết!"
- ✅ Có log: "À, user A lúc 14:30 đặt bàn 5 mà bàn chỉ 4 chỗ!"

### 🎯 **Ví dụ thực tế:**

**File log: `logs/app-2024-10-28.log`**
```
[14:30:01 INFO] User 'admin' logged in successfully
[14:30:15 INFO] Creating walk-in booking: Table=5, Guests=6
[14:30:16 WARN] Validation failed: 6 guests > 4 capacity
[14:30:16 ERROR] Walk-in booking failed: Số khách vượt sức chứa
[14:35:20 INFO] Order #62 status changed: Đang phục vụ → Đã thanh toán
```

→ Khi giảng viên hỏi: "Sao đặt bàn lỗi?"
→ Bạn: "Dạ vì số khách nhiều hơn sức chứa thầy ạ!" (Có log chứng minh!)

### 📝 **Cách làm:**

**Bước 1: Cài package**
```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

**Bước 2: Config trong Program.cs**
```csharp
using Serilog;

// Đầu file Program.cs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

**Bước 3: Sử dụng trong Controller**
```csharp
public class OrderManagementController : Controller
{
    private readonly ILogger<OrderManagementController> _logger;

    public OrderManagementController(ILogger<OrderManagementController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
    {
        _logger.LogInformation("Adding item {MenuItemId} x{Quantity} to Order {OrderId}", 
            request.MenuItemId, request.Quantity, request.OrderId);

        try
        {
            // ... logic ...
            _logger.LogInformation("Item added successfully");
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add item to order");
            return Json(new { success = false });
        }
    }
}
```

---

## 📊 TỔNG KẾT CẢI TIẾN

| Cải tiến | Thời gian | Tác động | Độ khó |
|----------|-----------|----------|--------|
| 1. Swagger | ✅ **Đã xong** | +0.5 điểm | ⭐ Dễ |
| 2. README | 1-2 giờ | +0.5 điểm | ⭐⭐ TB |
| 3. Fix Warnings | ✅ **Đã xong** | +0.3 điểm | ⭐ Dễ |
| 4. Password Hash | 30 phút | +0.2 điểm | ⭐ Dễ |
| 5. Exception Handler | 1 giờ | +0.3 điểm | ⭐⭐ TB |
| 6. Logging | 1 giờ | +0.2 điểm | ⭐⭐ TB |
| **TỔNG** | **~4 giờ** | **+2 điểm** | - |

---

## 🎯 LỘ TRÌNH ƯU TIÊN

### **NGAY BÂY GIỜ (1 giờ):**
1. ✅ Swagger - **Đã xong**
2. ✅ Fix 2 warnings async - **Đã xong**
3. Fix warning null reference (5 phút)
4. Test build → 0 warnings (5 phút)

### **HÔM NAY (2-3 giờ):**
5. Viết README đẹp (1 giờ)
6. Chụp screenshots (30 phút)
7. Password hashing check (30 phút)
8. Global Exception Handler (1 giờ)

### **NGÀY MAI (1-2 giờ):**
9. Logging system (1 giờ)
10. Test toàn bộ app (30 phút)
11. Commit + Push Git (10 phút)

---

## ✅ CHECKLIST NỘP BÀI

### **Code Quality**
- [x] Swagger API docs
- [ ] 0 warnings
- [ ] 0 errors
- [ ] Password hashing toàn bộ
- [ ] Global exception handler
- [ ] Logging system

### **Documentation**
- [ ] README đầy đủ
- [ ] Screenshots đẹp
- [ ] API docs (Swagger)
- [ ] Database schema
- [ ] Setup instructions

### **Features**
- [x] Frontend React
- [x] Backend ASP.NET Core
- [x] Database + Migrations
- [x] CRUD operations
- [x] Authentication
- [x] Booking system
- [x] Order management
- [x] Payment
- [x] Walk-in booking
- [x] Order history

### **Deployment**
- [x] Docker support
- [x] Seed data
- [ ] Production config

---

**Chúc bạn cải tiến thành công! 🚀**

Nếu cần giúp đỡ, hãy hỏi từng bước một!









