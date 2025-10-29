using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [Route("test-data")]
    public class TestDataController : Controller
    {
        private readonly RestaurantDbContext _context;

        public TestDataController(RestaurantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employeeCount = await _context.Employees.CountAsync();
            var menuItemCount = await _context.MenuItems.CountAsync();
            var tableCount = await _context.Tables.CountAsync();
            var settingsCount = await _context.RestaurantSettings.CountAsync();

            var employees = await _context.Employees.Take(3).ToListAsync();
            var menuItems = await _context.MenuItems.Take(3).ToListAsync();
            var tables = await _context.Tables.Take(3).ToListAsync();

            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Kiểm tra Database</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }}
        .container {{ background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        h1 {{ color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 10px; }}
        h2 {{ color: #34495e; margin-top: 30px; }}
        .stat {{ display: inline-block; margin: 15px 20px 15px 0; padding: 15px 25px; background: #3498db; color: white; border-radius: 5px; font-size: 18px; }}
        .success {{ background: #27ae60; }}
        .warning {{ background: #e74c3c; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 15px; }}
        th, td {{ padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background-color: #34495e; color: white; }}
        tr:hover {{ background-color: #f5f5f5; }}
        .back-btn {{ display: inline-block; margin-top: 30px; padding: 12px 25px; background: #3498db; color: white; text-decoration: none; border-radius: 5px; }}
        .back-btn:hover {{ background: #2980b9; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>🔍 Kiểm tra Database - Nhà Hàng</h1>
        
        <h2>📊 Thống kê tổng quan:</h2>
        <div class='stat {(employeeCount > 0 ? "success" : "warning")}'>👥 Nhân viên: {employeeCount}</div>
        <div class='stat {(menuItemCount > 0 ? "success" : "warning")}'>🍽️ Món ăn: {menuItemCount}</div>
        <div class='stat {(tableCount > 0 ? "success" : "warning")}'>🪑 Bàn ăn: {tableCount}</div>
        <div class='stat {(settingsCount > 0 ? "success" : "warning")}'>⚙️ Cài đặt: {settingsCount}</div>

        <h2>👥 Danh sách nhân viên (Top 3):</h2>
        <table>
            <tr><th>ID</th><th>Họ tên</th><th>Username</th><th>Email</th><th>Vai trò</th><th>Trạng thái</th></tr>
            {string.Join("", employees.Select(e => $"<tr><td>{e.Id}</td><td>{e.FullName}</td><td>{e.Username}</td><td>{e.Email}</td><td><strong>{e.Role}</strong></td><td>{(e.IsActive ? "✅ Hoạt động" : "❌ Khóa")}</td></tr>"))}
        </table>

        <h2>🍽️ Danh sách món ăn (Top 3):</h2>
        <table>
            <tr><th>ID</th><th>Tên món</th><th>Danh mục</th><th>Giá</th><th>Mô tả</th></tr>
            {string.Join("", menuItems.Select(m => $"<tr><td>{m.Id}</td><td><strong>{m.Name}</strong></td><td>{m.Category}</td><td>{m.Price:N0} đ</td><td>{m.Description}</td></tr>"))}
        </table>

        <h2>🪑 Danh sách bàn (Top 3):</h2>
        <table>
            <tr><th>ID</th><th>Tên bàn</th><th>Tầng</th><th>Sức chứa</th><th>Mô tả</th><th>Trạng thái</th></tr>
            {string.Join("", tables.Select(t => $"<tr><td>{t.Id}</td><td><strong>{t.Name}</strong></td><td>{t.Floor}</td><td>{t.Capacity} người</td><td>{t.Description}</td><td>{(t.IsActive ? "✅ Hoạt động" : "❌ Đóng")}</td></tr>"))}
        </table>

        <a href='/Auth/Login' class='back-btn'>🔐 Đăng nhập Admin</a>
        <a href='/' class='back-btn' style='background: #27ae60;'>🏠 Về trang chủ</a>
    </div>
</body>
</html>";

            return Content(html, "text/html");
        }
    }
}



