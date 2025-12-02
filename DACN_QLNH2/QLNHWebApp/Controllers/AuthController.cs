using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QLNHWebApp.Models;
using QLNHWebApp.Services;

namespace QLNHWebApp.Controllers
{
    /// <summary>
    /// Controller xử lý Authentication (Login/Logout) cho Admin Panel
    /// Sử dụng Cookie-based Authentication với ASP.NET Core Identity
    /// Flow: Login form → Verify password (BCrypt) → Tạo Claims → SignIn → Cookie được gửi browser
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // MVC Controller, không hiển thị trong Swagger
    public class AuthController : Controller
    {
        // Database context để query bảng Employees
        private readonly RestaurantDbContext _context;

        // Service seed dữ liệu mẫu (admin mặc định, menu, bàn...)
        private readonly DataSeederService _dataSeeder;

        // Constructor: Dependency Injection
        public AuthController(RestaurantDbContext context, DataSeederService dataSeeder)
        {
            _context = context;
            _dataSeeder = dataSeeder;
        }

        /// <summary>
        /// Hiển thị trang đăng nhập (GET)
        /// Route: /Auth/Login
        /// Tự động seed dữ liệu nếu lần đầu chạy ứng dụng
        /// </summary>
        /// <returns>View Login.cshtml</returns>
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // Gọi DataSeeder để đảm bảo có dữ liệu mẫu (admin, menu, tables...)
            // SeedAsync kiểm tra xem đã seed chưa, nếu rồi thì không làm gì
            await _dataSeeder.SeedAsync();

            // Trả về View Login.cshtml (form login)
            return View();
        }

        /// <summary>
        /// Xử lý form login (POST)
        /// Route: POST /Auth/Login
        /// Flow: Validate input → Query Employees → Verify password (BCrypt) → Create Claims → SignIn
        /// </summary>
        /// <param name="username">Tên đăng nhập (username)</param>
        /// <param name="password">Mật khẩu (plain text - sẽ hash với BCrypt)</param>
        /// <param name="rememberMe">"Ghi nhớ tôi" - Cookie 30 ngày nếu true, 8h nếu false</param>
        /// <returns>Redirect đến Dashboard hoặc trang tương ứng với Role</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Bảo vệ CSRF attack
        public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
        {
            // BƯỚC 1: VALIDATION - Kiểm tra input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                // Thêm error vào ModelState (hiển thị trong View)
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin đăng nhập.");
                return View(); // Trả lại form với error
            }

            // BƯỚC 2: QUERY DATABASE - Tìm Employee với username
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == username && e.IsActive);
            // IsActive: Kiểm tra tài khoản có bị khóa không

            // BƯỚC 3: VERIFY PASSWORD - Kiểm tra mật khẩu với BCrypt
            if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
            {
                // BCrypt.Verify: So sánh password plain text với hash trong DB
                // Không tìm thấy user HOẶC password sai → Thông báo lỗi chung (security best practice)
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }

            // BƯỚC 4: TẠO CLAIMS - Lưu thông tin user vào cookie
            // Claims: Thông tin user (ID, tên, role) được mã hóa trong cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()), // User ID
                new Claim(ClaimTypes.Name, employee.Username),                 // Username
                new Claim("FullName", employee.FullName),                      // Tên đầy đủ
                new Claim(ClaimTypes.Role, employee.Role)                      // Role (Admin, Nhân viên, Đầu bếp)
            };

            // Tạo ClaimsIdentity với authentication scheme "AdminAuth" (đã config trong Program.cs)
            var claimsIdentity = new ClaimsIdentity(claims, "AdminAuth");

            // AuthenticationProperties: Cấu hình cookie
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe, // Cookie lưu dài hạn (survive browser restart)
                ExpiresUtc = rememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)  // "Ghi nhớ tôi" → 30 ngày
                    : DateTimeOffset.UtcNow.AddHours(8)  // Không check → 8 tiếng
            };

            // BƯỚC 5: SIGN IN - Tạo cookie và gửi về browser
            await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(claimsIdentity), authProperties);
            // Browser tự động lưu cookie và gửi kèm trong mọi request tiếp theo

            // Redirect theo role
            return employee.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Nhân viên" => RedirectToAction("Index", "OrderManagement"),
                "Đầu bếp" => RedirectToAction("Index", "AdminMenu"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        /// <summary>
        /// Xử lý đăng xuất (POST hoặc GET)
        /// Route: /Auth/Logout
        /// Flow: SignOut (xóa cookie) → Xóa Session → Redirect Login
        /// </summary>
        /// <returns>Redirect đến trang Login</returns>
        [HttpPost]
        [HttpGet] // Cho phép GET để user có thể logout bằng URL /Auth/Logout
        public async Task<IActionResult> Logout()
        {
            // BƯỚC 1: Xóa authentication cookie
            await HttpContext.SignOutAsync("AdminAuth");
            // Browser sẽ xóa cookie, user không còn authenticated

            // BƯỚC 2: Xóa tất cả session data (giỏ hàng, booking info...)
            HttpContext.Session.Clear();

            // BƯỚC 3: Hiển thị thông báo thành công (survive 1 redirect)
            TempData["SuccessMessage"] = "Đăng xuất thành công!";

            // BƯỚC 4: Redirect về trang đăng nhập
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}