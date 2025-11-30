using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    /// <summary>
    /// Controller quản lý đặt bàn cho Admin/Staff
    /// Chức năng: Xem, xác nhận, từ chối, chỉnh sửa booking
    /// Chuyển đổi booking thành order khi xác nhận
    /// </summary>
    [Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminAndStaff")] // Chỉ Admin và Staff mới truy cập được
    [ApiExplorerSettings(IgnoreApi = true)] // Không hiển thị trong Swagger API docs
    public class AdminBookingController : Controller
    {
        // Database context để truy vấn và cập nhật dữ liệu
        private readonly RestaurantDbContext _context;

        // Constructor: Dependency Injection tự động inject DbContext
        public AdminBookingController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Action hiển thị danh sách booking (Trang chính quản lý đặt bàn)
        /// Route: /AdminBooking/Index
        /// Có filter: status, search (tên/SĐT), date
        /// </summary>
        public async Task<IActionResult> Index(string? status, string? search, DateTime? date)
        {
            // BƯỚC 1: Tạo query lấy danh sách bookings
            // AsQueryable() cho phép thêm điều kiện WHERE động
            var bookingsQuery = _context.TableBookings
                .Include(tb => tb.Table) // Load thông tin bàn
                .Include(tb => tb.OrderItems)  // Load món ăn đã đặt
                    .ThenInclude(oi => oi.MenuItem) // Load chi tiết món ăn
                .AsQueryable(); // Chưa execute, vẫn ở dạng IQueryable

            // BƯỚC 2: Áp dụng các bộ lọc (nếu có)
            // Filter theo trạng thái (Pending, Confirmed, Cancelled, ...)
            if (!string.IsNullOrEmpty(status))
            {
                bookingsQuery = bookingsQuery.Where(tb => tb.Status == status);
                ViewData["StatusFilter"] = status; // Lưu để hiển thị lại trong View
            }

            // Filter theo tên khách hoặc số điện thoại
            if (!string.IsNullOrEmpty(search))
            {
                bookingsQuery = bookingsQuery.Where(tb =>
                    tb.CustomerName.Contains(search) || // LIKE %search%
                    tb.Phone.Contains(search));
                ViewData["SearchFilter"] = search;
            }

            // Filter theo ngày đặt
            if (date.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(tb => tb.BookingDate.Date == date.Value.Date);
                ViewData["DateFilter"] = date.Value.ToString("yyyy-MM-dd");
            }

            // BƯỚC 3: Execute query và sắp xếp
            var bookings = await bookingsQuery
                .OrderByDescending(tb => tb.BookingDate) // Sắp xếp ngày mới nhất trước
                .ThenByDescending(tb => tb.Id) // Nếu cùng ngày thì ID lớn hơn trước
                .ToListAsync(); // Execute và load vào memory

            // BƯỚC 4: Tính toán thống kê (hiển thị trên dashboard)
            var totalBookings = await _context.TableBookings.CountAsync(); // Tổng số booking
            var todayBookings = await _context.TableBookings
                .CountAsync(tb => tb.BookingDate.Date == DateTime.Today); // Booking hôm nay
            var pendingBookings = await _context.TableBookings
                .CountAsync(tb => tb.Status == "Pending"); // Đang chờ xác nhận
            var confirmedBookings = await _context.TableBookings
                .CountAsync(tb => tb.Status == "Confirmed"); // Đã xác nhận

            // Truyền thống kê sang View qua ViewBag (dynamic property)
            ViewBag.TotalBookings = totalBookings;
            ViewBag.TodayBookings = todayBookings;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.ConfirmedBookings = confirmedBookings;

            // Trả về View với danh sách bookings làm Model
            return View(bookings);
        }

        /// <summary>
        /// Hiển thị chi tiết 1 booking cụ thể
        /// Route: /AdminBooking/Details/{id}
        /// Ví dụ: /AdminBooking/Details/5
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            // Tìm booking theo ID, kèm theo related data
            var booking = await _context.TableBookings
                .Include(tb => tb.Table) // Load thông tin bàn
                .Include(tb => tb.OrderItems)  // Load danh sách món đã đặt
                    .ThenInclude(oi => oi.MenuItem) // Load chi tiết món ăn
                .FirstOrDefaultAsync(tb => tb.Id == id); // Lấy booking đầu tiên khớp ID

            // Nếu không tìm thấy, trả về 404 Not Found page
            if (booking == null)
            {
                return NotFound();
            }

            // Trả về View Details với booking làm Model
            return View(booking);
        }

        /// <summary>
        /// API cập nhật trạng thái booking (AJAX call)
        /// Endpoint: POST /AdminBooking/UpdateStatus
        /// Params: id (booking ID), status (Pending/Confirmed/Cancelled/...)
        /// </summary>
        [HttpPost] // Chỉ chấp nhận POST method
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            // Tìm booking theo ID (không cần Include vì chỉ update Status)
            var booking = await _context.TableBookings.FindAsync(id);
            if (booking == null)
            {
                // Trả về JSON error (dùng cho AJAX)
                return Json(new { success = false, message = "Không tìm thấy đặt bàn!" });
            }

            // Cập nhật trạng thái
            booking.Status = status;
            await _context.SaveChangesAsync(); // Lưu vào database

            // Trả về JSON success
            return Json(new
            {
                success = true,
                message = $"Đã cập nhật trạng thái thành '{GetStatusName(status)}'."
            });
        }

        /// <summary>
        /// API xác nhận booking và tự động chuyển thành Order (QUAN TRỌNG)
        /// Endpoint: POST /AdminBooking/Confirm
        /// Flow: TableBooking → Order (booking biến thành đơn hàng thực tế)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] ConfirmRequest request)
        {
            // VALIDATION: Kiểm tra dữ liệu đầu vào
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            // BƯỚC 1: Lấy thông tin booking kèm món ăn đã đặt
            var booking = await _context.TableBookings
                .Include(tb => tb.Table) // Thông tin bàn
                .Include(tb => tb.OrderItems)  // Danh sách món đã đặt
                    .ThenInclude(oi => oi.MenuItem) // Chi tiết món
                .FirstOrDefaultAsync(tb => tb.Id == request.Id);

            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đặt bàn!" });
            }

            // Cập nhật trạng thái booking (optional, vì sẽ xóa sau)
            booking.Status = "Confirmed";

            // BƯỚC 2: Tính tổng tiền từ món ăn đã đặt
            // Σ(Giá × Số lượng) của tất cả món
            decimal totalPrice = booking.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            // BƯỚC 3: TẠO ORDER MỚI (chuyển đổi booking thành order thực tế)
            // Đây là bước QUAN TRỌNG: từ "đặt trước" → "đang phục vụ"
            var order = new Order
            {
                CustomerName = booking.CustomerName, // Copy thông tin khách
                Phone = booking.Phone,
                Date = booking.BookingDate, // Ngày đặt
                Time = booking.BookingTime, // Giờ đặt
                Guests = booking.Guests, // Số khách
                Note = booking.Note, // Ghi chú
                TableId = booking.TableId, // Bàn nào
                TotalPrice = totalPrice, // Tổng tiền đã tính
                Status = "Đang phục vụ" // Trạng thái mới: đang phục vụ
            };

            // Thêm Order vào database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Phải save để có Order.Id

            // BƯỚC 4: CHUYỂN MÓN ĂN TỪ BOOKING SANG ORDER
            if (booking.OrderItems != null && booking.OrderItems.Any())
            {
                // Duyệt qua từng món trong booking
                foreach (var bookingItem in booking.OrderItems)
                {
                    // Tạo OrderItem mới cho Order
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id, // Link tới Order mới tạo
                        MenuItemId = bookingItem.MenuItemId, // Món gì
                        Quantity = bookingItem.Quantity, // Số lượng
                        Price = bookingItem.Price, // Giá (snapshot)
                        TableBookingId = null  // Xóa link với TableBooking (vì chuyển sang Order rồi)
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // XÓA CÁC ORDERITEMS CŨ (vì đã copy sang Order rồi)
                _context.OrderItems.RemoveRange(booking.OrderItems);
                await _context.SaveChangesAsync();
            }

            // BƯỚC 5: XÓA BOOKING (vì đã chuyển thành Order rồi)
            // Booking chỉ là bước trung gian, sau khi confirm → xóa đi
            _context.TableBookings.Remove(booking);
            await _context.SaveChangesAsync();

            // Trả về JSON success với ID đơn hàng mới
            return Json(new
            {
                success = true,
                message = "Đã xác nhận đặt bàn và tạo đơn hàng thành công!",
                orderId = order.Id // Trả về ID để redirect hoặc xử lý tiếp
            });
        }

        // POST: Từ chối booking nhanh
        [HttpPost]
        public async Task<IActionResult> Reject([FromBody] RejectRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            var booking = await _context.TableBookings
                .Include(tb => tb.OrderItems)
                .FirstOrDefaultAsync(tb => tb.Id == request.Id);

            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đặt bàn!" });
            }

            // XÓA CÁC ORDERITEMS TRƯỚC
            if (booking.OrderItems != null && booking.OrderItems.Any())
            {
                _context.OrderItems.RemoveRange(booking.OrderItems);
            }

            // XÓA BOOKING
            _context.TableBookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã từ chối và xóa đặt bàn!" });
        }

        // GET: Form chỉnh sửa booking
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _context.TableBookings
                .Include(tb => tb.Table)
                .Include(tb => tb.OrderItems)  // Load món ăn
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Load danh sách bàn để chọn
            ViewBag.Tables = await _context.Tables
                .Where(t => t.IsActive)
                .OrderBy(t => t.Floor)
                .ThenBy(t => t.Name)
                .ToListAsync();

            // Load danh sách món ăn để chọn
            ViewBag.MenuItems = await _context.MenuItems
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();

            return View(booking);
        }

        // POST: Lưu chỉnh sửa booking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TableBooking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            var existingBooking = await _context.TableBookings.FindAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            // Update fields
            existingBooking.CustomerName = booking.CustomerName;
            existingBooking.Phone = booking.Phone;
            existingBooking.BookingDate = booking.BookingDate;
            existingBooking.BookingTime = booking.BookingTime;
            existingBooking.Guests = booking.Guests;
            existingBooking.Note = booking.Note;
            existingBooking.TableId = booking.TableId;
            existingBooking.Status = booking.Status;

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thông tin đặt bàn đã được cập nhật thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật thông tin đặt bàn.";

                // Reload tables for dropdown
                ViewBag.Tables = await _context.Tables
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Floor)
                    .ThenBy(t => t.Name)
                    .ToListAsync();

                return View(booking);
            }
        }

        // POST: Xóa booking
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.TableBookings
                .FirstOrDefaultAsync(tb => tb.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.TableBookings.Remove(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt bàn đã được xóa thành công.";
            return RedirectToAction(nameof(Index));
        }

        // POST: API - Tạo đặt bàn trực tiếp (walk-in)
        [HttpPost]
        public async Task<IActionResult> CreateWalkIn([FromBody] WalkInBookingRequest request)
        {
            try
            {
                // VALIDATION 1: Kiểm tra bàn có tồn tại không
                var table = await _context.Tables.FindAsync(request.TableId);
                if (table == null)
                {
                    return Json(new { success = false, message = "Bàn không tồn tại!" });
                }

                // VALIDATION 2: Kiểm tra số khách không vượt quá sức chứa bàn
                if (request.Guests > table.Capacity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Bàn {table.Name} chỉ chứa tối đa {table.Capacity} người! Bạn đang đặt {request.Guests} người."
                    });
                }

                // VALIDATION 3: Kiểm tra bàn đã bị đặt chưa (trong các đơn đang hoạt động)
                var isTableOccupied = await _context.Orders
                    .AnyAsync(o => o.TableId == request.TableId &&
                                   (o.Status == "Đang phục vụ" || o.Status == "Chưa thanh toán" || o.Status == "Đã xác nhận"));

                if (isTableOccupied)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Bàn {table.Name} đã có khách! Vui lòng chọn bàn khác."
                    });
                }

                // Tạo order trực tiếp cho khách walk-in
                var order = new Order
                {
                    CustomerName = request.CustomerName,
                    Phone = request.Phone,
                    Date = DateTime.Now,
                    Time = DateTime.Now.ToString("HH:mm"),
                    Guests = request.Guests,
                    TableId = request.TableId,
                    Status = "Đang phục vụ",  // Đang phục vụ ngay
                    TotalPrice = 0,
                    Note = request.Note
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = $"Đã tạo đơn cho {request.CustomerName} tại {table.Name}!",
                    orderId = order.Id
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Helper method
        private string GetStatusName(string status)
        {
            return status switch
            {
                "Pending" => "Chờ xác nhận",
                "Confirmed" => "Đã xác nhận",
                "Cancelled" => "Đã hủy",
                "Completed" => "Hoàn thành",
                _ => status
            };
        }

        // ===== QUẢN LÝ MÓN ĂN TRONG BOOKING =====

        // POST: Thêm món vào booking
        [HttpPost]
        public async Task<IActionResult> AddItemToBooking([FromBody] AddItemToBookingRequest request)
        {
            try
            {
                var booking = await _context.TableBookings
                    .Include(tb => tb.OrderItems)
                    .FirstOrDefaultAsync(tb => tb.Id == request.BookingId);

                if (booking == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đặt bàn!" });
                }

                var menuItem = await _context.MenuItems.FindAsync(request.MenuItemId);
                if (menuItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn!" });
                }

                OrderItem orderItem;
                // Kiểm tra xem món đã có trong booking chưa
                var existingItem = booking.OrderItems.FirstOrDefault(oi => oi.MenuItemId == request.MenuItemId);
                if (existingItem != null)
                {
                    // Nếu đã có, tăng số lượng
                    existingItem.Quantity += request.Quantity;
                    orderItem = existingItem;
                }
                else
                {
                    // Nếu chưa có, thêm mới
                    orderItem = new OrderItem
                    {
                        TableBookingId = request.BookingId,
                        MenuItemId = request.MenuItemId,
                        Quantity = request.Quantity,
                        Price = menuItem.Price
                    };
                    booking.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                // Trả về thông tin món vừa thêm/cập nhật
                return Json(new
                {
                    success = true,
                    message = "Đã thêm món vào đặt bàn!",
                    item = new
                    {
                        id = orderItem.Id,
                        menuItemName = menuItem.Name,
                        price = orderItem.Price,
                        quantity = orderItem.Quantity,
                        total = orderItem.Price * orderItem.Quantity
                    },
                    isNew = existingItem == null
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Xóa món khỏi booking
        [HttpPost]
        public async Task<IActionResult> RemoveItemFromBooking([FromBody] RemoveItemFromBookingRequest request)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId && oi.TableBookingId == request.BookingId);

                if (orderItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn!" });
                }

                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa món khỏi đặt bàn!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // POST: Cập nhật số lượng món trong booking
        [HttpPost]
        public async Task<IActionResult> UpdateItemQuantityInBooking([FromBody] UpdateItemQuantityRequest request)
        {
            try
            {
                var orderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.Id == request.OrderItemId && oi.TableBookingId == request.BookingId);

                if (orderItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy món ăn!" });
                }

                if (request.Quantity <= 0)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0!" });
                }

                orderItem.Quantity = request.Quantity;
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã cập nhật số lượng!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }

    // DTO classes for API requests
    public class ConfirmRequest
    {
        public int Id { get; set; }
    }

    public class RejectRequest
    {
        public int Id { get; set; }
        public string? Reason { get; set; }
    }

    public class AddItemToBookingRequest
    {
        public int BookingId { get; set; }
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class RemoveItemFromBookingRequest
    {
        public int BookingId { get; set; }
        public int OrderItemId { get; set; }
    }

    public class UpdateItemQuantityRequest
    {
        public int BookingId { get; set; }
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class WalkInBookingRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Guests { get; set; }
        public int TableId { get; set; }
        public string? Note { get; set; }
    }
}