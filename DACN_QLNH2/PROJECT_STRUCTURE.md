# 📁 Cấu trúc dự án chi tiết

## 🎯 Tổng quan

Dự án được tổ chức theo mô hình **Frontend-Backend separation** với cấu trúc rõ ràng, dễ bảo trì.

---

## 🏗️ Backend Structure (QLNHWebApp/)

### **Controllers/**
```
Controllers/
├── Api/                                 # 🔌 RESTful API Controllers
│   ├── ContactApiController.cs          # API liên hệ, phản hồi
│   ├── MenuApiController.cs             # API thực đơn (GET)
│   ├── OrderApiController.cs            # API đơn hàng (CRUD)
│   └── TableApiController.cs            # API bàn & đặt bàn (POST BookTable)
│
├── Admin Controllers (MVC)              # 👨‍💼 Controllers cho Admin Dashboard
│   ├── AdminController.cs               # Dashboard, Employees, Statistics
│   ├── AdminBookingController.cs        # Quản lý đặt bàn (Confirm/Reject/WalkIn)
│   ├── AdminCustomerController.cs       # Quản lý khách hàng
│   ├── AdminMenuController.cs           # CRUD thực đơn
│   ├── OrderManagementController.cs     # Quản lý đơn hàng (Edit/History)
│   └── SettingsController.cs            # Cài đặt hệ thống
│
└── Client Controllers (MVC)             # 🏠 Controllers cho khách hàng
    ├── HomeController.cs                # Trang chủ
    ├── BookingController.cs             # Flow đặt bàn
    ├── TableController.cs               # Chọn bàn, tầng
    ├── PaymentController.cs             # Thanh toán & xác nhận
    └── AuthController.cs                # Login/Logout

```

#### **Phân chia trách nhiệm:**
- **`Api/`**: Chỉ trả về JSON (cho React frontend hoặc mobile app)
- **MVC Controllers**: Trả về Views (Razor pages) cho admin dashboard

---

### **Models/**
```
Models/
├── DTOs/                                # 📦 Data Transfer Objects (NEW!)
│   ├── BookingDTOs.cs                   # TableBookingRequest, WalkInBookingRequest, etc.
│   ├── OrderDTOs.cs                     # AddItemRequest, UpdateQuantityRequest, etc.
│   └── PaymentDTOs.cs                   # ConfirmPaymentRequest, PaymentResponse
│
├── RestaurantModels.cs                  # 🗄️ Entity Models
│   ├── Employee                         # Nhân viên
│   ├── Table                            # Bàn ăn
│   ├── MenuItem                         # Món ăn
│   ├── TableBooking                     # Đặt bàn (chưa xác nhận)
│   ├── Order                            # Đơn hàng (đã xác nhận)
│   ├── OrderItem                        # Chi tiết món trong đơn
│   └── RestaurantSettings               # Cài đặt
│
├── RestaurantDbContext.cs               # EF Core DbContext
└── ErrorViewModel.cs                    # View model cho error pages
```

#### **Lợi ích của DTOs:**
✅ **Tách biệt** business logic khỏi database entities
✅ **Validation** tập trung tại một chỗ
✅ **API documentation** rõ ràng hơn (Swagger)
✅ **Bảo mật** - không expose toàn bộ entity ra ngoài

---

### **Services/**
```
Services/
└── DataSeederService.cs                 # Seed dữ liệu mẫu khi khởi động
```

**Kế hoạch mở rộng:**
- `ITableService.cs` / `TableService.cs` - Business logic cho bàn ăn
- `IOrderService.cs` / `OrderService.cs` - Business logic cho đơn hàng
- `IEmailService.cs` / `EmailService.cs` - Gửi email xác nhận

---

### **Views/** (Razor Pages)
```
Views/
├── Admin/                               # Admin Dashboard Views
│   ├── Dashboard.cshtml                 # Tổng quan, thống kê
│   ├── Employees.cshtml                 # Quản lý nhân viên
│   ├── Orders.cshtml                    # Danh sách đơn hàng
│   └── Menu/                            # CRUD thực đơn
│
├── AdminBooking/                        # Quản lý đặt bàn
│   ├── Index.cshtml                     # Danh sách booking (+ Walk-in modal)
│   ├── Details.cshtml                   # Chi tiết booking
│   └── Edit.cshtml                      # Sửa booking
│
├── AdminMenu/                           # CRUD thực đơn
│   ├── Index.cshtml
│   ├── Create.cshtml
│   ├── Edit.cshtml
│   └── Details.cshtml
│
├── OrderManagement/                     # Quản lý đơn hàng
│   ├── Index.cshtml                     # Đơn hiện thời
│   ├── History.cshtml                   # Lịch sử hóa đơn
│   ├── Edit.cshtml                      # Sửa đơn (Add/Remove items)
│   └── PrintInvoice.cshtml              # In hóa đơn
│
├── Payment/                             # Thanh toán
│   ├── Index.cshtml                     # Trang thanh toán
│   └── Success.cshtml                   # Thanh toán thành công
│
├── Auth/
│   └── Login.cshtml                     # Đăng nhập admin
│
└── Shared/                              # Layouts
    ├── _Layout.cshtml                   # Layout cho client
    ├── _AdminLayout.cshtml              # Layout cho admin
    └── _ValidationScriptsPartial.cshtml
```

---

### **wwwroot/** (Static Files)
```
wwwroot/
├── assets/                              # React build output (auto-generated)
│   ├── index-*.js
│   └── index-*.css
│
├── images/                              # Hình ảnh món ăn
│   ├── boluclac.jpg
│   ├── goicuon.jpg
│   └── menu/                            # Upload từ admin
│
├── css/
│   └── payment.css                      # Custom CSS cho trang payment
│
├── lib/                                 # Client-side libraries
│   ├── jquery/
│   └── jquery-validation/
│
├── index.html                           # React app entry point
└── favicon.ico
```

---

## ⚛️ Frontend Structure (Restaurant Management Web App/)

```
Restaurant Management Web App/
├── src/
│   ├── components/                      # React Components
│   │   ├── BookingPage.tsx              # 🎫 Trang đặt bàn
│   │   ├── MenuPage.tsx                 # 🍔 Trang thực đơn
│   │   ├── TableSelection.tsx           # 🪑 Chọn bàn
│   │   ├── PaymentForm.tsx              # 💳 Form thanh toán
│   │   └── ... (55 components)
│   │
│   ├── services/
│   │   └── api.ts                       # Axios instance, API calls
│   │
│   ├── types/
│   │   └── index.ts                     # TypeScript interfaces
│   │
│   ├── data/
│   │   └── mockData.ts                  # Mock data cho development
│   │
│   ├── hooks/
│   │   └── useCart.ts                   # Custom hooks (shopping cart)
│   │
│   ├── App.tsx                          # Main App component
│   ├── main.tsx                         # Entry point
│   └── index.css                        # Global styles (TailwindCSS)
│
├── build/                               # Production build (npm run build)
├── package.json                         # NPM dependencies
├── vite.config.ts                       # Vite configuration
└── tsconfig.json                        # TypeScript configuration
```

---

## 🔄 Data Flow

### **1. Client Đặt Bàn:**
```
React BookingPage 
    → POST /api/tableapi/BookTable (with OrderItems)
        → TableApiController 
            → Save TableBooking + OrderItems to DB
                → Return JSON { success: true, bookingId: ... }
```

### **2. Admin Xác Nhận Booking:**
```
Admin Dashboard 
    → POST /AdminBooking/Confirm/{id}
        → AdminBookingController
            → Create Order from TableBooking
            → Copy OrderItems from Booking to Order
            → Delete TableBooking
                → Redirect to OrderManagement/Edit/{orderId}
```

### **3. Admin Sửa Đơn Hàng:**
```
OrderManagement/Edit/{id}
    → POST /OrderManagement/AddItem
        → Add new OrderItem to Order
        → Recalculate TotalPrice
            → Return JSON { success: true, newTotalPrice: ... }
                → Reload page to show updated items
```

---

## 🗄️ Database Schema

```
Tables (SQLite)
├── Employees               # Nhân viên (admin/staff)
├── Tables                  # Bàn ăn (tầng, sức chứa)
├── MenuItems               # Thực đơn (món ăn, giá)
├── TableBookings           # Đặt bàn (chưa xác nhận) → Xóa sau khi confirm
├── Orders                  # Đơn hàng (đã xác nhận)
├── OrderItems              # Chi tiết món trong đơn
│   ├── FK: OrderId         # Thuộc về Order nào
│   ├── FK: MenuItemId      # Món gì
│   └── FK: TableBookingId  # (Nullable) Nếu là booking chưa confirm
└── RestaurantSettings      # Cài đặt (tên, địa chỉ, thuế VAT)
```

**Quan hệ:**
- `OrderItem.OrderId` → `Order.Id` (1-N)
- `OrderItem.MenuItemId` → `MenuItem.Id` (N-1)
- `OrderItem.TableBookingId` → `TableBooking.Id` (N-1, nullable)
- `Order.TableId` → `Table.Id` (N-1)

---

## 📂 Files Đã Xóa (Cleanup)

- ❌ `TestController.cs` - File test không cần thiết
- ❌ `add_test_items_to_booking13.sql` - SQL test
- ❌ `Views/Payment/Index_Simple.cshtml` - View cũ không dùng

---

## 🆕 Files Mới Thêm

- ✅ `Models/DTOs/BookingDTOs.cs`
- ✅ `Models/DTOs/OrderDTOs.cs`
- ✅ `Models/DTOs/PaymentDTOs.cs`
- ✅ `.gitignore` - Ignore build artifacts, node_modules
- ✅ `README.md` - Documentation tổng thể
- ✅ `PROJECT_STRUCTURE.md` - File này

---

## 🎯 Best Practices

### **✅ Đã áp dụng:**
1. **Separation of Concerns** - API / MVC tách biệt
2. **DTOs Pattern** - Tách request/response khỏi entities
3. **RESTful API** - Đúng chuẩn HTTP methods
4. **Password Hashing** - BCrypt cho bảo mật
5. **Global Exception Handler** - Bắt lỗi toàn cục
6. **Swagger Documentation** - API docs tự động

### **🔄 Kế hoạch cải thiện:**
1. **Dependency Injection** - Tách business logic sang Services
2. **Repository Pattern** - Abstraction layer cho database
3. **Logging** - Serilog cho production
4. **Unit Testing** - xUnit + Moq
5. **CI/CD** - GitHub Actions

---

**📝 Cập nhật lần cuối**: 28/10/2024  
**👤 Người duy trì**: DACN Team









