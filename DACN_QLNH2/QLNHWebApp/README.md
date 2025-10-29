# QLNHWebApp - Ứng dụng quản lý nhà hàng ASP.NET Core MVC

## Tính năng chính
- Trang chủ: Banner, slogan, menu điều hướng, tìm kiếm món ăn
- Thực đơn: Lọc, tìm kiếm, đặt món, xem chi tiết, đánh giá món ăn
- Giỏ hàng: Thêm/xóa/sửa số lượng món, tự động tính tổng tiền
- Đặt bàn: Form đặt bàn, lưu vào SQL Server
- Thanh toán: Hiển thị chi tiết đơn, chọn phương thức thanh toán, lưu trạng thái
- Giới thiệu, Liên hệ: Thông tin nhà hàng, form liên hệ lưu vào DB
- Seed data mẫu cho MenuItems
- Script SQL tạo database, bảng, dữ liệu mẫu

## Hướng dẫn sử dụng
1. **Cấu hình kết nối SQL Server**
   - Sửa chuỗi kết nối trong `appsettings.json` cho phù hợp với SQL Server của bạn.
2. **Khởi tạo database**
   - Chạy script `Scripts/init_qlnhdb.sql` trên SQL Server để tạo database, bảng và dữ liệu mẫu.
3. **Chạy ứng dụng**
   - Build: `dotnet build`
   - Chạy: `dotnet run`
   - Truy cập: `https://localhost:5001` hoặc `http://localhost:5000`
4. **Chức năng**
   - Đặt món, đặt bàn, thanh toán, gửi liên hệ, đánh giá món ăn, quản lý giỏ hàng.

## Công nghệ sử dụng
- ASP.NET Core MVC 9
- Entity Framework Core
- SQL Server
- Bootstrap 5

## Thư mục chính
- `Controllers/` - Xử lý logic các trang
- `Models/` - Định nghĩa dữ liệu, DbContext
- `Views/` - Giao diện Razor Pages
- `Scripts/` - Script SQL tạo database, seed data
- `Helpers/` - Extension cho session

## Ghi chú
- Để giỏ hàng hoạt động, project đã cấu hình session trong `Program.cs`.
- Nếu muốn thêm seed data, chỉnh sửa file SQL hoặc `OnModelCreating` trong DbContext.
- Để sử dụng tính năng đánh giá món ăn, đăng nhập không bắt buộc.

---

Mọi thắc mắc vui lòng liên hệ qua trang Liên hệ của ứng dụng.