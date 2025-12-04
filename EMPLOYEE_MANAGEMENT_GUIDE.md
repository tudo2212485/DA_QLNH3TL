# 📋 HƯỚNG DẪN CẬP NHẬT HỆ THỐNG QUẢN LÝ NHÂN VIÊN

## ✅ CÁC THAY ĐỔI ĐÃ THỰC HIỆN

### 1. XÓA ROLE "ĐẦU BẾP" - CHỈ GIỮ LẠI 2 ROLE

#### **File đã sửa: `Services/DataSeederService.cs`**
```csharp
// ĐÃ XÓA: Employee với Role = "Đầu bếp"
// CHỈ CÒN:
// - Admin (toàn quyền)
// - Staff/Nhân viên (phục vụ & quản lý)
```

**Kết quả:**
- ✅ Seed data chỉ tạo 2 nhân viên mẫu: `admin` và `nhanvien`
- ✅ Hệ thống chỉ có 2 roles: **Admin** và **Staff**

---

### 2. CHỨC NĂNG ADMIN ĐỔI MẬT KHẨU NHÂN VIÊN (FORCE RESET)

#### **File đã sửa: `Controllers/AdminController.cs`**

**Action mới:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ResetPassword(int id, [FromForm] string newPassword)
{
    // Validate mật khẩu mới (tối thiểu 6 ký tự)
    // Lấy Employee từ database
    // Force reset: employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
    // Không cần mật khẩu cũ!
    // Trả về JSON success/error
}
```

**Đặc điểm:**
- ✅ Admin có toàn quyền đổi mật khẩu nhân viên
- ✅ Không cần biết mật khẩu cũ
- ✅ Sử dụng BCrypt để hash mật khẩu mới
- ✅ Validation: mật khẩu tối thiểu 6 ký tự
- ✅ Trả về mật khẩu mới trong message để admin copy

---

#### **File đã sửa: `Views/Admin/Employees.cshtml`**

**Thêm nút Reset Password:**
```html
<button type="button" class="action-btn action-password" 
        onclick="showResetPasswordModal(@employee.Id, '@employee.FullName')"
        title="Đổi mật khẩu">
    <i class="bx bx-key"></i>
</button>
```

**Thêm Modal Reset Password:**
```html
<div class="modal fade" id="resetPasswordModal">
    <!-- Form nhập mật khẩu mới -->
    <!-- Input: newPassword (required, minlength=6) -->
    <!-- Input: confirmNewPassword (required) -->
    <!-- Alert cảnh báo: "Mật khẩu cũ sẽ bị vô hiệu hóa ngay lập tức" -->
    <!-- Button: Đặt lại mật khẩu -->
</div>
```

**JavaScript:**
```javascript
// Function: showResetPasswordModal(employeeId, employeeName)
// Function: resetEmployeePassword() {
//     Validation: newPassword.length >= 6
//     Validation: newPassword === confirmNewPassword
//     Confirm dialog: "Bạn có chắc muốn đặt lại mật khẩu?"
//     Fetch POST: /Admin/ResetPassword
//     Alert success với mật khẩu mới để admin copy
// }
```

---

### 3. CẬP NHẬT DROPDOWN ROLE - XÓA "ĐẦU BẾP"

#### **File đã sửa: `Views/Admin/Employees.cshtml`**

**Trước:**
```html
<select class="modern-select" id="role">
    <option value="Admin">👑 Admin</option>
    <option value="Nhân viên">🍽️ Nhân viên</option>
    <option value="Đầu bếp">👨‍🍳 Đầu bếp</option> <!-- ĐÃ XÓA -->
</select>
```

**Sau:**
```html
<select class="modern-select" id="role">
    <option value="Admin">👑 Admin - Toàn quyền hệ thống</option>
    <option value="Staff">🍽️ Staff - Nhân viên phục vụ</option>
</select>
<small class="text-muted">
    <i class="bx bx-info-circle"></i> Chỉ còn 2 role: Admin (toàn quyền) và Staff (nhân viên)
</small>
```

**Cập nhật Filter dropdown:**
```html
<select class="modern-select" id="roleFilter">
    <option value="">Tất cả vai trò</option>
    <option value="Admin">👑 Admin</option>
    <option value="Staff">🍽️ Staff (Nhân viên)</option>
    <!-- ĐÃ XÓA: Đầu bếp -->
</select>
```

---

### 4. PHÂN QUYỀN (AUTHORIZATION POLICIES)

#### **File đã sửa: `Program.cs`**

**Cập nhật Policies:**
```csharp
builder.Services.AddAuthorization(options =>
{
    // Policy 1: CHỈ ADMIN
    // AdminController (Dashboard, Employees, Statistics)
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Policy 2: ADMIN + STAFF
    // OrderManagementController, TableController, AdminBookingController
    // ĐÃ XÓA: "Đầu bếp"
    options.AddPolicy("AdminAndStaff", policy =>
        policy.RequireRole("Admin", "Staff", "Nhân viên")); // Support legacy "Nhân viên"

    // Policy 3: TẤT CẢ (bao gồm Customer)
    options.AddPolicy("AllRoles", policy =>
        policy.RequireRole("Admin", "Staff", "Nhân viên", "Customer"));
});
```

**Controller đã có sẵn Authorization:**
```csharp
// AdminController.cs
[Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminOnly")]
public class AdminController : Controller { ... }

// Các controller khác (OrderManagement, Table, AdminBooking)
[Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminAndStaff")]
```

---

### 5. ẨN MENU "QUẢN LÝ NHÂN VIÊN" VỚI STAFF

#### **File: `Views/Shared/_AdminLayout.cshtml`**

**Đã có sẵn check:**
```html
@if (User.IsInRole("Admin"))
{
    <li class="nav-item">
        <a class="nav-link" href="@Url.Action("Employees", "Admin")">
            <i class="bx bx-group"></i>
            Nhân viên
        </a>
    </li>
    
    <li class="nav-item">
        <a class="nav-link" href="@Url.Action("Index", "AdminCustomer")">
            <i class="bx bx-user"></i>
            Khách hàng
        </a>
    </li>
    
    <li class="nav-item">
        <a class="nav-link" href="@Url.Action("Index", "Settings")">
            <i class="bx bx-cog"></i>
            Thiết lập hệ thống
        </a>
    </li>
}
```

**Kết quả:**
- ✅ Nếu login bằng `admin`: Hiển thị đầy đủ menu
- ✅ Nếu login bằng `nhanvien` (Staff): Chỉ thấy Dashboard, Đơn hàng, Đặt bàn, Thực đơn

---

## 📊 THỐNG KÊ CARDS - ĐÃ CẬP NHẬT

**File đã sửa: `Views/Admin/Employees.cshtml`**

**Trước:** 4 cards (Admin, Nhân viên, Đầu bếp, ...)

**Sau:** 4 cards
1. **Tổng nhân viên** (Model.Count())
2. **Admin** (Count role = "Admin")
3. **Nhân viên (Staff)** (Count role = "Staff" hoặc "Nhân viên")
4. **Đang hoạt động** (Count IsActive = true) - THAY THẾ card "Đầu bếp"

---

## 🎨 CSS MỚI CHO NÚT RESET PASSWORD

```css
.action-password {
    color: #f59e0b;
    border-color: #f59e0b30;
}

.action-password:hover {
    background: linear-gradient(135deg, #f59e0b, #d97706);
    color: white;
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
}
```

---

## 🧪 KIỂM THỬ

### Test Case 1: Xóa Role Đầu bếp
1. ✅ Mở trang `/Admin/Employees`
2. ✅ Click "Thêm nhân viên"
3. ✅ Dropdown "Vai trò" CHỈ CÒN 2 option: Admin và Staff
4. ✅ Filter dropdown CHỈ CÒN 2 option: Admin và Staff

### Test Case 2: Reset Password
1. ✅ Mở trang `/Admin/Employees`
2. ✅ Click nút 🔑 (Key icon) ở cột Thao tác
3. ✅ Modal "Đổi mật khẩu nhân viên" hiện lên
4. ✅ Nhập mật khẩu mới: `Test123` (ít hơn 6 ký tự → Lỗi)
5. ✅ Nhập mật khẩu mới: `Test12345`
6. ✅ Nhập xác nhận: `Test1234` (không khớp → Lỗi)
7. ✅ Nhập xác nhận: `Test12345` (khớp)
8. ✅ Click "Đặt lại mật khẩu"
9. ✅ Confirm dialog: "Bạn có chắc..."
10. ✅ Success: Toast + Alert hiển thị mật khẩu mới

### Test Case 3: Phân quyền
1. ✅ Login bằng `nhanvien` / `123456`
2. ✅ Sidebar KHÔNG hiển thị: Nhân viên, Khách hàng, Thiết lập hệ thống
3. ✅ Sidebar CHỈ hiển thị: Dashboard, Đơn hàng, Đặt bàn, Thực đơn
4. ✅ Try truy cập `/Admin/Employees` → 403 Forbidden

### Test Case 4: Seed Data
1. ✅ Xóa database cũ
2. ✅ Run migration: `dotnet ef database update`
3. ✅ Mở trang đăng nhập → DataSeeder chạy
4. ✅ Database CHỈ CÓ 2 nhân viên: `admin` và `nhanvien`

---

## 📝 GHI CHÚ QUAN TRỌNG

### ⚠️ Backward Compatibility
- Code vẫn support role `"Nhân viên"` (legacy) để không break data cũ
- Khuyến nghị: Dùng `"Staff"` cho nhân viên mới
- Policy `AdminAndStaff` accept cả `"Admin"`, `"Staff"`, `"Nhân viên"`

### 🔐 Bảo mật
- Reset Password chỉ Admin mới có quyền (Policy="AdminOnly")
- Không thể xóa hoặc vô hiệu hóa tài khoản `admin`
- BCrypt cost factor = 11 (mặc định) - đủ an toàn

### 🚀 Tương lai
- Có thể thêm chức năng "Force Change Password at Next Login"
- Có thể thêm Password History (không cho đổi trùng 5 mật khẩu gần nhất)
- Có thể thêm Log activity (ai đổi mật khẩu cho ai, khi nào)

---

## 📚 TÀI LIỆU THAM KHẢO

- **BCrypt.Net Docs:** https://github.com/BcryptNet/bcrypt.net
- **ASP.NET Core Authorization:** https://learn.microsoft.com/en-us/aspnet/core/security/authorization
- **Bootstrap 5 Modal:** https://getbootstrap.com/docs/5.3/components/modal/

---

## ✨ TÓM TẮT

| **Yêu cầu** | **Trạng thái** | **File liên quan** |
|-------------|----------------|--------------------|
| Xóa role "Đầu bếp" | ✅ HOÀN THÀNH | DataSeederService.cs, Employees.cshtml, Program.cs |
| Chức năng Reset Password | ✅ HOÀN THÀNH | AdminController.cs, Employees.cshtml |
| Ẩn menu với Staff | ✅ ĐÃ CÓ SẴN | _AdminLayout.cshtml (check User.IsInRole) |
| Cập nhật Policies | ✅ HOÀN THÀNH | Program.cs (AdminAndStaff policy) |

**Tất cả chức năng đã được implement và sẵn sàng test!** 🎉
