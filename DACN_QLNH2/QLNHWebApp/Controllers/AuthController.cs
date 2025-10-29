using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QLNHWebApp.Models;
using QLNHWebApp.Services;

namespace QLNHWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly DataSeederService _dataSeeder;

        public AuthController(RestaurantDbContext context, DataSeederService dataSeeder)
        {
            _context = context;
            _dataSeeder = dataSeeder;
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            // Ensure seed data exists
            await _dataSeeder.SeedAsync();
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, bool rememberMe = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin đăng nhập.");
                return View();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == username && e.IsActive);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.PasswordHash))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim("FullName", employee.FullName),
                new Claim(ClaimTypes.Role, employee.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "AdminAuth");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync("AdminAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect theo role
            return employee.Role switch
            {
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Nhân viên" => RedirectToAction("Index", "OrderManagement"),
                "Đầu bếp" => RedirectToAction("Index", "AdminMenu"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminAuth");
            
            // Xóa session
            HttpContext.Session.Clear();
            
            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = "Đăng xuất thành công!";
            
            // Redirect về trang đăng nhập
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}