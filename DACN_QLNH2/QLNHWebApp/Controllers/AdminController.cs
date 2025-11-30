using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminOnly")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminController : Controller
    {
        private readonly RestaurantDbContext _context;

        public AdminController(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            // Basic Statistics
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán")
                .SumAsync(o => o.TotalPrice);
            var totalCustomers = await _context.Orders
                .Where(o => !string.IsNullOrEmpty(o.CustomerName))
                .Select(o => o.Phone) // Use phone as unique identifier
                .Distinct()
                .CountAsync();

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.UserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            ViewBag.FullName = User.FindFirst("FullName")?.Value;

            // === DATA FOR CHARTS ===

            // 1. Revenue by Month (Last 6 months) - IMPROVED LOGIC
            // Tạo danh sách đầy đủ 6 tháng gần nhất (kể cả tháng có doanh thu = 0)
            var last6Months = new List<(int Year, int Month, string Label)>();
            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                last6Months.Add((date.Year, date.Month, $"Tháng {date.Month}/{date.Year}"));
            }

            // Lấy doanh thu thực tế từ database
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            var actualRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán" && o.Date >= sixMonthsAgo)
                .GroupBy(o => new { o.Date.Year, o.Date.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .ToListAsync();

            // Merge data: Tạo array đầy đủ 6 tháng, tháng nào không có dữ liệu = 0
            var revenueLabels = new List<string>();
            var revenueData = new List<decimal>();
            var orderCountData = new List<int>();

            foreach (var month in last6Months)
            {
                revenueLabels.Add($"\"{month.Label}\"");

                var data = actualRevenue.FirstOrDefault(r => r.Year == month.Year && r.Month == month.Month);
                if (data != null)
                {
                    revenueData.Add(data.Revenue);
                    orderCountData.Add(data.OrderCount);
                }
                else
                {
                    revenueData.Add(0);
                    orderCountData.Add(0);
                }
            }

            ViewBag.RevenueLabels = string.Join(",", revenueLabels);
            ViewBag.RevenueData = string.Join(",", revenueData);
            ViewBag.OrderCountData = string.Join(",", orderCountData);

            // 2. Orders by Status (Pie Chart)
            var ordersByStatus = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            ViewBag.StatusLabels = string.Join(",", ordersByStatus.Select(s => $"\"{s.Status}\""));
            ViewBag.StatusData = string.Join(",", ordersByStatus.Select(s => s.Count));

            // 3. Top 5 Menu Items (Bar Chart)
            var topMenuItems = await _context.OrderItems
                .GroupBy(oi => oi.MenuItemId)
                .Select(g => new
                {
                    MenuItemId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToListAsync();

            var menuItemNames = new List<string>();
            foreach (var item in topMenuItems)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                menuItemNames.Add(menuItem?.Name ?? "Unknown");
            }

            ViewBag.TopMenuLabels = string.Join(",", menuItemNames.Select(name => $"\"{name}\""));
            ViewBag.TopMenuQuantities = string.Join(",", topMenuItems.Select(x => x.TotalQuantity));
            ViewBag.TopMenuRevenue = string.Join(",", topMenuItems.Select(x => x.TotalRevenue));

            // 4. Today's stats
            var today = DateTime.Today;
            var todayOrders = await _context.Orders.Where(o => o.Date == today).CountAsync();
            var todayRevenue = await _context.Orders
                .Where(o => o.Date == today && o.Status == "Đã thanh toán")
                .SumAsync(o => o.TotalPrice);

            ViewBag.TodayOrders = todayOrders;
            ViewBag.TodayRevenue = todayRevenue;

            return View();
        }

        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetail(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Employees()
        {
            var employees = await _context.Employees
                .OrderBy(e => e.Id)
                .ToListAsync();

            return View(employees);
        }

        // API: Get Employee by ID
        [HttpGet]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Không tìm thấy nhân viên" });
            }

            return Json(new
            {
                id = employee.Id,
                fullName = employee.FullName,
                username = employee.Username,
                email = employee.Email,
                role = employee.Role,
                createdAt = employee.CreatedAt.ToString("dd/MM/yyyy"),
                isActive = employee.IsActive
            });
        }

        // API: Create Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmployee([FromForm] string fullName, [FromForm] string username,
            [FromForm] string email, [FromForm] string role, [FromForm] string password)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });
                }

                // Check if username already exists
                if (await _context.Employees.AnyAsync(e => e.Username == username))
                {
                    return Json(new { success = false, message = "Username đã tồn tại" });
                }

                // Check if email already exists
                if (await _context.Employees.AnyAsync(e => e.Email == email))
                {
                    return Json(new { success = false, message = "Email đã tồn tại" });
                }

                // Validate password length
                if (password.Length < 6)
                {
                    return Json(new { success = false, message = "Mật khẩu phải có ít nhất 6 ký tự" });
                }

                // Create new employee
                var employee = new Employee
                {
                    FullName = fullName,
                    Username = username,
                    Email = email,
                    Role = role,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Thêm nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // API: Update Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmployee([FromForm] int id, [FromForm] string fullName,
            [FromForm] string username, [FromForm] string email, [FromForm] string role)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                // Check if username is taken by another employee
                if (await _context.Employees.AnyAsync(e => e.Username == username && e.Id != id))
                {
                    return Json(new { success = false, message = "Username đã tồn tại" });
                }

                // Check if email is taken by another employee
                if (await _context.Employees.AnyAsync(e => e.Email == email && e.Id != id))
                {
                    return Json(new { success = false, message = "Email đã tồn tại" });
                }

                // Update employee
                employee.FullName = fullName;
                employee.Username = username;
                employee.Email = email;
                employee.Role = role;

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // API: Toggle Employee Status (Activate/Deactivate)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEmployeeStatus(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                // Don't allow deactivating admin account
                if (employee.Username == "admin")
                {
                    return Json(new { success = false, message = "Không thể vô hiệu hóa tài khoản admin" });
                }

                employee.IsActive = !employee.IsActive;
                await _context.SaveChangesAsync();

                string status = employee.IsActive ? "kích hoạt" : "vô hiệu hóa";
                return Json(new { success = true, message = $"Đã {status} nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // API: Delete Employee
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                // Don't allow deleting admin account
                if (employee.Username == "admin")
                {
                    return Json(new { success = false, message = "Không thể xóa tài khoản admin" });
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa nhân viên thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}