using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class OrdersApiController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public OrdersApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng với phân trang
        /// </summary>
        /// <param name="page">Số trang (mặc định: 1)</param>
        /// <param name="pageSize">Số lượng mỗi trang (mặc định: 10)</param>
        /// <param name="status">Lọc theo trạng thái</param>
        /// <param name="search">Tìm kiếm theo tên hoặc SĐT</param>
        /// <returns>Danh sách đơn hàng và thông tin phân trang</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<Order>), 200)]
        public async Task<ActionResult<PaginatedResult<Order>>> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
            }

            // Search by customer name or phone
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => 
                    o.CustomerName.Contains(search) || 
                    o.Phone.Contains(search));
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PaginatedResult<Order>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                HasPrevious = page > 1,
                HasNext = page < (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin 1 đơn hàng theo ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Order), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng" });
            }

            return Ok(order);
        }

        /// <summary>
        /// Thống kê đơn hàng
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<ActionResult<object>> GetStatistics()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán")
                .SumAsync(o => o.TotalPrice);
            var pendingOrders = await _context.Orders
                .Where(o => o.Status == "Chờ xác nhận" || o.Status == "Đang phục vụ")
                .CountAsync();
            var completedOrders = await _context.Orders
                .Where(o => o.Status == "Đã thanh toán")
                .CountAsync();

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                pendingOrders,
                completedOrders,
                averageOrderValue = completedOrders > 0 ? totalRevenue / completedOrders : 0
            });
        }
    }

    /// <summary>
    /// Model cho kết quả phân trang
    /// </summary>
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}

