using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;
using QLNHWebApp.Controllers;

namespace QLNHWebApp.Controllers.Api
{
    /// <summary>
    /// API Controller xử lý các yêu cầu đặt bàn và đơn hàng
    /// Sử dụng RESTful API pattern với JSON request/response
    /// </summary>
    [ApiController] // Đánh dấu đây là API Controller, tự động xử lý model validation và response format
    [Route("api/[controller]")] // Định nghĩa route: /api/OrderApi
    public class OrderApiController : ControllerBase
    {
        // Dependency Injection: Entity Framework DbContext để truy cập database
        private readonly RestaurantDbContext _context;

        // Constructor: ASP.NET Core tự động inject RestaurantDbContext
        public OrderApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// API lưu thông tin đặt bàn vào Session (bước trung gian)
        /// Endpoint: POST /api/OrderApi/save-booking-info
        /// Mục đích: Lưu tạm thông tin khách hàng trước khi chọn món
        /// </summary>
        [HttpPost("save-booking-info")] // Chỉ chấp nhận HTTP POST method
        public IActionResult SaveBookingInfo([FromBody] BookingRequest request) // [FromBody]: Lấy dữ liệu từ JSON body
        {
            // Kiểm tra validation: ModelState tự động validate dựa trên Data Annotations
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Trả về 400 Bad Request nếu dữ liệu không hợp lệ

            // Tạo object BookingInfo để lưu vào session
            var bookingInfo = new BookingInfo
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Date = request.Date.ToString("yyyy-MM-dd"), // Format ngày theo chuẩn ISO
                Time = request.Time,
                Guests = request.Guests,
                Note = request.Note ?? "" // Null coalescing: nếu null thì gán rỗng
            };

            // Lưu vào Session sử dụng Extension method tự tạo (serialize thành JSON)
            HttpContext.Session.SetObjectAsJson("BookingInfo", bookingInfo);

            // Trả về 200 OK với message JSON
            return Ok(new { message = "Booking info saved successfully" });
        }

        /// <summary>
        /// API tạo booking đơn giản (chỉ đặt bàn, chưa chọn món)
        /// Endpoint: POST /api/OrderApi/booking
        /// Use case: Khách đặt bàn trước, chưa quyết định món ăn
        /// </summary>
        [HttpPost("booking")]
        public async Task<ActionResult<Order>> CreateBooking([FromBody] BookingRequest request)
        {
            // Validate dữ liệu đầu vào
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Tạo Order mới (trong hệ thống này Order = Booking + Order)
            var order = new Order
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Date = request.Date,
                Time = request.Time,
                Guests = request.Guests,
                Note = request.Note,
                TotalPrice = 0, // Chưa có món ăn nên tổng tiền = 0
                Status = "Chờ xác nhận", // Trạng thái ban đầu
                OrderItems = new List<OrderItem>() // Khởi tạo danh sách rỗng
            };

            // Thêm vào DbContext (chưa lưu database)
            _context.Orders.Add(order);
            // Lưu vào database (async để không block thread)
            await _context.SaveChangesAsync();

            // Trả về 201 Created với Location header trỏ đến GetOrder endpoint
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        /// <summary>
        /// API tạo đơn hàng hoàn chỉnh (có thông tin khách + món ăn + thanh toán)
        /// Endpoint: POST /api/OrderApi/checkout
        /// Use case: Khách đã chọn xong món, tiến hành checkout
        /// </summary>
        [HttpPost("checkout")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // BƯỚC 1: Tính tổng tiền
            // Lấy danh sách món ăn từ database dựa trên MenuItemId trong request
            var menuItems = await _context.MenuItems
                .Where(m => request.Items.Select(i => i.MenuItemId).Contains(m.Id))
                .ToListAsync();

            // Tính tổng tiền = Σ(Giá món × Số lượng)
            decimal totalPrice = request.Items.Sum(item =>
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                return menuItem?.Price * item.Quantity ?? 0; // ?? 0: nếu null thì trả về 0
            });

            // BƯỚC 2: Tạo Order
            var order = new Order
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Date = request.Date,
                Time = request.Time,
                Guests = request.Guests,
                Note = request.Note,
                TotalPrice = totalPrice, // Tổng tiền đã tính
                Status = request.PaymentMethod, // Trạng thái = Phương thức thanh toán
                TableId = request.TableId, // Bàn nào (có thể null nếu mang về)
                OrderItems = new List<OrderItem>() // Khởi tạo list rỗng
            };

            // Thêm Order vào database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // QUAN TRỌNG: Phải save để có Order.Id

            // BƯỚC 3: Thêm OrderItems (chi tiết món ăn)
            foreach (var item in request.Items)
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                if (menuItem != null) // Chỉ thêm nếu món tồn tại
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id, // Foreign key tới Order vừa tạo
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = menuItem.Price // Lưu giá tại thời điểm đặt (snapshot)
                    });
                }
            }

            // Lưu lại OrderItems
            await _context.SaveChangesAsync();

            // Trả về 201 Created với order vừa tạo
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        /// <summary>
        /// API lấy thông tin 1 đơn hàng cụ thể (theo ID)
        /// Endpoint: GET /api/OrderApi/{id}
        /// Ví dụ: GET /api/OrderApi/5
        /// </summary>
        [HttpGet("{id}")] // {id} là route parameter
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            // Eager Loading: Include để load cả related entities (OrderItems, MenuItem, Table)
            var order = await _context.Orders
                .Include(o => o.OrderItems) // Load OrderItems của Order này
                .ThenInclude(oi => oi.MenuItem) // Sau đó load MenuItem của mỗi OrderItem
                .Include(o => o.Table) // Load Table information
                .FirstOrDefaultAsync(o => o.Id == id); // Lấy Order đầu tiên có Id khớp (hoặc null)

            // Nếu không tìm thấy, trả về 404 Not Found
            if (order == null)
                return NotFound();

            // Trả về 200 OK với Order object (tự động serialize thành JSON)
            return order;
        }

        /// <summary>
        /// API lấy danh sách tất cả đơn hàng
        /// Endpoint: GET /api/OrderApi
        /// Sắp xếp theo ngày giảm dần (mới nhất lên trước)
        /// </summary>
        [HttpGet] // Không có route parameter
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // Lấy tất cả orders, bao gồm cả related data
            return await _context.Orders
                .Include(o => o.OrderItems) // Load OrderItems
                .ThenInclude(oi => oi.MenuItem) // Load MenuItem cho mỗi OrderItem
                .Include(o => o.Table) // Load Table
                .OrderByDescending(o => o.Date) // Sắp xếp ngày mới nhất trước
                .ToListAsync(); // Execute query và convert thành List
        }
    }

    #region DTO Classes (Data Transfer Objects)

    /// <summary>
    /// DTO cho request đặt bàn đơn giản
    /// Chứa thông tin khách hàng cơ bản
    /// </summary>
    public class BookingRequest
    {
        public string CustomerName { get; set; } = string.Empty; // Tên khách
        public string Phone { get; set; } = string.Empty; // Số điện thoại
        public DateTime Date { get; set; } // Ngày đặt
        public string Time { get; set; } = string.Empty; // Giờ đặt (format: HH:mm)
        public int Guests { get; set; } // Số khách
        public string? Note { get; set; } // Ghi chú (có thể null)
    }

    /// <summary>
    /// DTO cho request checkout đầy đủ
    /// Bao gồm thông tin khách + danh sách món ăn + phương thức thanh toán
    /// </summary>
    public class CheckoutRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int Guests { get; set; }
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "Tiền mặt", "Chuyển khoản", etc.
        public int? TableId { get; set; } // Nullable: có thể không có bàn (mang về)
        public List<OrderItemRequest> Items { get; set; } = new(); // Danh sách món đặt
    }

    /// <summary>
    /// DTO cho từng món ăn trong order
    /// Chỉ cần MenuItemId và Quantity
    /// </summary>
    public class OrderItemRequest
    {
        public int MenuItemId { get; set; } // ID món ăn
        public int Quantity { get; set; } // Số lượng
    }

    #endregion
}

