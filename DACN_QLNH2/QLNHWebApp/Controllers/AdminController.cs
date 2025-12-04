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
            var viewModel = new DashboardViewModel();

            // === USER INFO ===
            viewModel.UserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            viewModel.FullName = User.FindFirst("FullName")?.Value;

            // === KPI CARDS ===

            // 1. ĐƠN HÀNG THÁNG NÀY (FIX: đổi từ "Tổng đơn hàng" sang "Đơn hàng tháng này")
            var now = DateTime.Now;
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var nextMonthStart = currentMonthStart.AddMonths(1);

            // Đếm đơn hàng tháng hiện tại
            viewModel.OrdersThisMonth = await _context.Orders
                .Where(o => o.Date >= currentMonthStart && o.Date < nextMonthStart)
                .CountAsync();

            // Đếm đơn hàng tháng trước (để tính %)
            var lastMonthStart = currentMonthStart.AddMonths(-1);
            viewModel.OrdersLastMonth = await _context.Orders
                .Where(o => o.Date >= lastMonthStart && o.Date < currentMonthStart)
                .CountAsync();

            // Tính % tăng trưởng đơn hàng
            if (viewModel.OrdersLastMonth > 0)
            {
                viewModel.OrdersGrowthRate = ((decimal)(viewModel.OrdersThisMonth - viewModel.OrdersLastMonth)
                    / viewModel.OrdersLastMonth) * 100;
            }
            else
            {
                viewModel.OrdersGrowthRate = viewModel.OrdersThisMonth > 0 ? 100 : 0;
            }

            // 2. Tổng doanh thu (ALL TIME) - chỉ tính đơn "Đã thanh toán"
            viewModel.TotalRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán")
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            // 3. DOANH THU THÁNG NÀY (THAY CARD "Tổng khách hàng")
            // Doanh thu tháng hiện tại
            viewModel.CurrentMonthRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán"
                    && o.Date >= currentMonthStart
                    && o.Date < nextMonthStart)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            // Doanh thu tháng trước
            viewModel.LastMonthRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán"
                    && o.Date >= lastMonthStart
                    && o.Date < currentMonthStart)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            // Tính % tăng trưởng (Growth Rate)
            if (viewModel.LastMonthRevenue > 0)
            {
                viewModel.MonthlyGrowthRate = ((viewModel.CurrentMonthRevenue - viewModel.LastMonthRevenue)
                    / viewModel.LastMonthRevenue) * 100;
            }
            else
            {
                // Nếu tháng trước = 0, xử lý đặc biệt
                viewModel.MonthlyGrowthRate = viewModel.CurrentMonthRevenue > 0 ? 100 : 0;
            }

            // 4. DOANH THU HÔM NAY (FIX: so sánh .Date để tránh lỗi DateTime)
            var today = DateTime.Today;

            // FIX: Sử dụng .Date.Date để so sánh chỉ phần ngày (loại bỏ giờ/phút/giây)
            viewModel.TodayRevenue = await _context.Orders
                .Where(o => o.Date.Date == today && o.Status == "Đã thanh toán")
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            viewModel.TodayOrders = await _context.Orders
                .Where(o => o.Date.Date == today)
                .CountAsync();

            // === REVENUE CHART DATA (12 tháng gần nhất - FIX LOGIC) ===

            // Tạo danh sách đầy đủ 12 tháng gần nhất
            var last12Months = new List<(int Year, int Month, string Label)>();
            for (int i = 11; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                last12Months.Add((date.Year, date.Month, $"Tháng {date.Month}/{date.Year}"));
            }

            // Lấy doanh thu THỰC TẾ từ database (CHỈ ĐƠN "ĐÃ THANH TOÁN")
            var twelveMonthsAgo = DateTime.Now.AddMonths(-12);
            var actualRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán" && o.Date >= twelveMonthsAgo)
                .GroupBy(o => new { o.Date.Year, o.Date.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(o => o.TotalPrice),
                    OrderCount = g.Count()
                })
                .ToListAsync();

            // DATA FILLING: Merge data để đảm bảo có đủ 12 phần tử
            foreach (var month in last12Months)
            {
                viewModel.RevenueLabels.Add(month.Label);

                var data = actualRevenue.FirstOrDefault(r => r.Year == month.Year && r.Month == month.Month);
                if (data != null)
                {
                    viewModel.RevenueData.Add(data.Revenue);
                    viewModel.OrderCountData.Add(data.OrderCount);
                }
                else
                {
                    // Tháng không có dữ liệu = 0 (KHÔNG BỎ QUA)
                    viewModel.RevenueData.Add(0);
                    viewModel.OrderCountData.Add(0);
                }
            }

            // === ORDER STATUS DATA (Pie Chart) ===
            var ordersByStatus = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            viewModel.StatusLabels = ordersByStatus.Select(s => s.Status).ToList();
            viewModel.StatusData = ordersByStatus.Select(s => s.Count).ToList();

            // === TOP MENU ITEMS DATA (Bar Chart) ===
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

            foreach (var item in topMenuItems)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                viewModel.TopMenuLabels.Add(menuItem?.Name ?? "Unknown");
                viewModel.TopMenuQuantities.Add(item.TotalQuantity);
                viewModel.TopMenuRevenue.Add(item.TotalRevenue);
            }

            return View(viewModel);
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

        /// <summary>
        /// API: Reset Password - Admin đổi mật khẩu cho nhân viên (Force Reset)
        /// Admin có toàn quyền đổi mật khẩu mà không cần biết mật khẩu cũ
        /// </summary>
        /// <param name="id">ID nhân viên</param>
        /// <param name="newPassword">Mật khẩu mới</param>
        /// <returns>JSON kết quả</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, [FromForm] string newPassword)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    return Json(new { success = false, message = "Vui lòng nhập mật khẩu mới" });
                }

                if (newPassword.Length < 6)
                {
                    return Json(new { success = false, message = "Mật khẩu phải có ít nhất 6 ký tự" });
                }

                // Find employee
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy nhân viên" });
                }

                // Force reset password (không cần mật khẩu cũ)
                // Hash mật khẩu mới bằng BCrypt
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

                // Save changes
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Đã đặt lại mật khẩu cho nhân viên '{employee.FullName}' thành công. Mật khẩu mới: {newPassword}"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }
    }
}