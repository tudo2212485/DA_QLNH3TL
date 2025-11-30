using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminAndStaff")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class OrderManagementController : Controller
    {
        private readonly RestaurantDbContext _context;

        public OrderManagementController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: Danh sách đơn hàng đang hoạt động
        public async Task<IActionResult> Index()
        {
            var activeOrders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .Where(o => o.Status == "Đang phục vụ" || o.Status == "Đã xác nhận" || o.Status == "Chưa thanh toán")
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            // Set ViewBag statistics
            ViewBag.TotalOrders = activeOrders.Count;
            ViewBag.PendingPayment = activeOrders.Count(o => o.Status == "Chưa thanh toán");
            ViewBag.ServingOrders = activeOrders.Count(o => o.Status == "Đang phục vụ");
            ViewBag.TotalRevenue = activeOrders.Sum(o => o.TotalPrice);

            return View(activeOrders);
        }

        // GET: Chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Cập nhật trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                order.Status = status;

                // Nếu chuyển sang "Đang phục vụ", ghi nhận thời gian bắt đầu
                if (status == "Đang phục vụ" && order.ServiceStartTime == null)
                {
                    order.ServiceStartTime = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã cập nhật trạng thái thành '{status}'!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Thanh toán đơn hàng (endpoint cũ)
        [HttpPost]
        public async Task<IActionResult> Payment([FromBody] PaymentRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                // Cập nhật trạng thái và thời gian kết thúc phục vụ
                order.Status = "Đã thanh toán";
                order.ServiceEndTime = DateTime.Now;

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Thanh toán thành công!",
                    orderId = order.Id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public class PaymentRequest
        {
            public int OrderId { get; set; }
        }

        // POST: Thanh toán đơn hàng (endpoint mới)
        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            return await Payment(request);
        }

        // GET: Lịch sử hóa đơn (đã thanh toán) - WITH PAGINATION
        public async Task<IActionResult> History(string? search, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 20)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .Where(o => o.Status == "Đã thanh toán");

            // Tìm kiếm theo tên khách hàng hoặc số điện thoại
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.CustomerName.Contains(search) || o.Phone.Contains(search));
            }

            // Lọc theo ngày
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(o => o.Date <= toDate.Value);
            }

            // Count total BEFORE pagination
            var totalCount = await query.CountAsync();

            // Calculate statistics on full dataset
            var allOrders = await query.ToListAsync();
            ViewBag.TotalOrders = allOrders.Count;
            ViewBag.TotalRevenue = allOrders.Sum(o => o.TotalPrice);
            ViewBag.TotalCustomers = allOrders.Select(o => o.Phone).Distinct().Count();
            ViewBag.AverageOrderValue = allOrders.Any() ? allOrders.Average(o => o.TotalPrice) : 0;

            // Apply PAGINATION (server-side)
            var orders = await query
                .OrderByDescending(o => o.Date)
                .ThenByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pagination info
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(orders);
        }

        // GET: API endpoint to get order details for modal
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                    .Include(o => o.Table)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn!" });
                }

                var result = new
                {
                    success = true,
                    order = new
                    {
                        id = order.Id,
                        customerName = order.CustomerName,
                        phone = order.Phone,
                        date = order.Date,
                        time = order.Time,
                        guests = order.Guests,
                        tableName = order.Table?.Name,
                        totalPrice = order.TotalPrice,
                        status = order.Status,
                        note = order.Note,
                        items = order.OrderItems.Select(oi => new
                        {
                            menuItemName = oi.MenuItem?.Name ?? "Món ăn",
                            quantity = oi.Quantity,
                            price = oi.Price
                        }).ToList()
                    }
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Xóa đơn hàng (chỉ xóa đơn đã thanh toán hoặc đã hủy)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                if (order.Status != "Đã thanh toán" && order.Status != "Đã hủy")
                {
                    return Json(new { success = false, message = "Chỉ có thể xóa đơn hàng đã thanh toán hoặc đã hủy!" });
                }

                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa đơn hàng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Hủy đơn hàng
        [HttpPost]
        public async Task<IActionResult> Cancel(int orderId, string? reason)
        {
            try
            {
                var order = await _context.Orders.FindAsync(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                order.Status = "Đã hủy";
                if (!string.IsNullOrEmpty(reason))
                {
                    order.Note = (order.Note ?? "") + $"\nLý do hủy: {reason}";
                }
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã hủy đơn hàng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // GET: Chỉnh sửa đơn hàng
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Load menu items và tables để chọn
            ViewBag.MenuItems = await _context.MenuItems.ToListAsync();
            ViewBag.Tables = await _context.Tables.ToListAsync();

            return View(order);
        }

        // POST: Cập nhật đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Order order, List<int>? menuItemIds, List<int>? quantities)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            try
            {
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (existingOrder == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin cơ bản
                existingOrder.CustomerName = order.CustomerName;
                existingOrder.Phone = order.Phone;
                existingOrder.Guests = order.Guests;
                existingOrder.Note = order.Note;
                existingOrder.Status = order.Status;

                // Cập nhật OrderItems nếu có
                if (menuItemIds != null && quantities != null)
                {
                    // Xóa OrderItems cũ
                    _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                    // Thêm OrderItems mới
                    for (int i = 0; i < menuItemIds.Count; i++)
                    {
                        var menuItem = await _context.MenuItems.FindAsync(menuItemIds[i]);
                        if (menuItem != null && quantities[i] > 0)
                        {
                            existingOrder.OrderItems.Add(new OrderItem
                            {
                                MenuItemId = menuItemIds[i],
                                Quantity = quantities[i],
                                Price = menuItem.Price
                            });
                        }
                    }

                    // Tính lại tổng tiền
                    existingOrder.TotalPrice = existingOrder.OrderItems.Sum(oi => oi.Price * oi.Quantity);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ViewBag.MenuItems = await _context.MenuItems.ToListAsync();
                ViewBag.Tables = await _context.Tables.ToListAsync();
                return View(order);
            }
        }

        // GET: In hóa đơn
        public async Task<IActionResult> PrintInvoice(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Lấy thông tin nhà hàng
            var restaurant = await _context.RestaurantSettings.FirstOrDefaultAsync();
            ViewBag.Restaurant = restaurant;

            return View(order);
        }

        // POST: Thêm món vào đơn hàng
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddItemRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == request.OrderId);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                var menuItem = await _context.MenuItems.FindAsync(request.MenuItemId);
                if (menuItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn!" });
                }

                // Kiểm tra xem món đã có trong đơn chưa
                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.MenuItemId == request.MenuItemId);
                if (existingItem != null)
                {
                    // Nếu đã có, tăng số lượng
                    existingItem.Quantity += request.Quantity;
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    order.OrderItems.Add(new OrderItem
                    {
                        MenuItemId = request.MenuItemId,
                        Quantity = request.Quantity,
                        Price = menuItem.Price
                    });
                }

                // Cập nhật tổng tiền
                order.TotalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Đã thêm món vào đơn hàng!",
                    totalPrice = order.TotalPrice
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Request model for AddItem
        public class AddItemRequest
        {
            public int OrderId { get; set; }
            public int MenuItemId { get; set; }
            public int Quantity { get; set; }
        }

        // POST: Xóa món khỏi đơn hàng
        [HttpPost]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveItemRequest request)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId);

                if (orderItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn!" });
                }

                var order = orderItem.Order;
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                _context.OrderItems.Remove(orderItem);

                // Cập nhật lại tổng tiền
                order.TotalPrice = order.OrderItems.Where(oi => oi.Id != request.OrderItemId).Sum(oi => oi.Price * oi.Quantity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa món khỏi đơn hàng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public class RemoveItemRequest
        {
            public int OrderItemId { get; set; }
        }

        // POST: Cập nhật số lượng món
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .Include(oi => oi.Order)
                    .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId);

                if (orderItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn!" });
                }

                if (request.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
                }

                orderItem.Quantity = request.Quantity;

                // Cập nhật lại tổng tiền
                var order = orderItem.Order;
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                order.TotalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã cập nhật số lượng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public class UpdateQuantityRequest
        {
            public int OrderItemId { get; set; }
            public int Quantity { get; set; }
        }

        // POST: Đổi bàn
        [HttpPost]
        public async Task<IActionResult> ChangeTable([FromBody] ChangeTableRequest request)
        {
            try
            {
                var order = await _context.Orders.FindAsync(request.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng!" });
                }

                var newTable = await _context.Tables.FindAsync(request.NewTableId);
                if (newTable == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy bàn mới!" });
                }

                order.TableId = request.NewTableId;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = $"Đã đổi sang {newTable.Name}!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        public class ChangeTableRequest
        {
            public int OrderId { get; set; }
            public int NewTableId { get; set; }
        }
    }
}
