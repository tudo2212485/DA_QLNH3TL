using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    public class AdminCustomerController : Controller
    {
        private readonly RestaurantDbContext _context;

        public AdminCustomerController(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, string? role)
        {
            // Lấy tất cả orders và group by phone
            var ordersData = await _context.Orders
                .GroupBy(o => o.Phone)
                .Select(g => new
                {
                    Phone = g.Key,
                    CustomerName = g.First().CustomerName,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(o => o.TotalPrice),
                    LastOrderDate = g.Max(o => o.Date),
                    LastStatus = g.OrderByDescending(o => o.Date).First().Status
                })
                .ToListAsync();

            // Convert to CustomerViewModel
            var customersQuery = ordersData.Select(o => new CustomerViewModel
            {
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                Email = "", // Email không có trong Order model hiện tại
                TotalOrders = o.TotalOrders,
                TotalSpent = o.TotalSpent,
                LastOrderDate = o.LastOrderDate,
                Status = o.LastStatus ?? "Active"
            }).AsQueryable();

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                customersQuery = customersQuery.Where(c => 
                    c.CustomerName.Contains(search) || 
                    c.Phone.Contains(search));
                ViewData["SearchFilter"] = search;
            }

            // Filter by role/status
            if (!string.IsNullOrEmpty(role))
            {
                customersQuery = customersQuery.Where(c => c.Status == role);
                ViewData["RoleFilter"] = role;
            }

            var customers = customersQuery
                .OrderByDescending(c => c.LastOrderDate)
                .ToList();

            // Calculate statistics
            var totalCustomers = customers.Count();
            var activeCustomers = customers.Count(c => c.Status == "Active" || c.Status == "Confirmed");
            var newCustomers = customers.Count(c => c.LastOrderDate >= DateTime.Today.AddDays(-30));
            var vipCustomers = customers.Count(c => c.TotalSpent >= 1000000); // VIP nếu chi tiêu >= 1M

            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.ActiveCustomers = activeCustomers;
            ViewBag.NewCustomers = newCustomers;
            ViewBag.VipCustomers = vipCustomers;

            return View(customers);
        }

        public async Task<IActionResult> Details(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return NotFound();
            }

            var customerOrders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Where(o => o.Phone == phone)
                .OrderByDescending(o => o.Date)
                .ToListAsync();

            if (!customerOrders.Any())
            {
                return NotFound();
            }

            var customer = new CustomerDetailsViewModel
            {
                CustomerName = customerOrders.First().CustomerName,
                Phone = phone,
                TotalOrders = customerOrders.Count,
                TotalSpent = customerOrders.Sum(o => o.TotalPrice),
                FirstOrderDate = customerOrders.Min(o => o.Date),
                LastOrderDate = customerOrders.Max(o => o.Date),
                Orders = customerOrders
            };

            return View(customer);
        }
    }

    public class CustomerViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastOrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CustomerDetailsViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime FirstOrderDate { get; set; }
        public DateTime LastOrderDate { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}