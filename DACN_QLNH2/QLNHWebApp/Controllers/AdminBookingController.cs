using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    public class AdminBookingController : Controller
    {
        private readonly RestaurantDbContext _context;

        public AdminBookingController(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status, string? search, DateTime? date)
        {
            var bookingsQuery = _context.TableBookings
                .Include(tb => tb.Table)
                .Include(tb => tb.OrderItems)  // Load món ăn để hiển thị trong danh sách
                    .ThenInclude(oi => oi.MenuItem)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                bookingsQuery = bookingsQuery.Where(tb => tb.Status == status);
                ViewData["StatusFilter"] = status;
            }

            // Filter by search (customer name or phone)
            if (!string.IsNullOrEmpty(search))
            {
                bookingsQuery = bookingsQuery.Where(tb => 
                    tb.CustomerName.Contains(search) || 
                    tb.Phone.Contains(search));
                ViewData["SearchFilter"] = search;
            }

            // Filter by date
            if (date.HasValue)
            {
                bookingsQuery = bookingsQuery.Where(tb => tb.BookingDate.Date == date.Value.Date);
                ViewData["DateFilter"] = date.Value.ToString("yyyy-MM-dd");
            }

            var bookings = await bookingsQuery
                .OrderByDescending(tb => tb.BookingDate)
                .ThenByDescending(tb => tb.Id)
                .ToListAsync();

            // Calculate statistics
            var totalBookings = await _context.TableBookings.CountAsync();
            var todayBookings = await _context.TableBookings
                .CountAsync(tb => tb.BookingDate.Date == DateTime.Today);
            var pendingBookings = await _context.TableBookings
                .CountAsync(tb => tb.Status == "Pending");
            var confirmedBookings = await _context.TableBookings
                .CountAsync(tb => tb.Status == "Confirmed");

            ViewBag.TotalBookings = totalBookings;
            ViewBag.TodayBookings = todayBookings;
            ViewBag.PendingBookings = pendingBookings;
            ViewBag.ConfirmedBookings = confirmedBookings;

            return View(bookings);
        }

        // GET: Chi tiết booking
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _context.TableBookings
                .Include(tb => tb.Table)
                .Include(tb => tb.OrderItems)  // Load món ăn đã đặt
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Cập nhật trạng thái (Xác nhận/Từ chối/Hủy)
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var booking = await _context.TableBookings.FindAsync(id);
            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đặt bàn!" });
            }

            booking.Status = status;
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = $"Đã cập nhật trạng thái thành '{GetStatusName(status)}'." 
            });
        }

        // POST: Xác nhận booking nhanh
        [HttpPost]
        public async Task<IActionResult> Confirm([FromBody] ConfirmRequest request)
        {
            if (request == null || request.Id <= 0)
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            var booking = await _context.TableBookings
                .Include(tb => tb.Table)
                .Include(tb => tb.OrderItems)  // Load món ăn đã đặt
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == request.Id);
                
            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đặt bàn!" });
            }

            booking.Status = "Confirmed";
            
            // Tính tổng tiền từ món ăn đã đặt
            decimal totalPrice = booking.OrderItems.Sum(oi => oi.Price * oi.Quantity);
            
            // TẠO ORDER TỰ ĐỘNG KHI XÁC NHẬN BOOKING
            var order = new Order
            {
                CustomerName = booking.CustomerName,
                Phone = booking.Phone,
                Date = booking.BookingDate,
                Time = booking.BookingTime,
                Guests = booking.Guests,
                Note = booking.Note,
                TableId = booking.TableId,
                TotalPrice = totalPrice,
                Status = "Đang phục vụ"
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            // COPY MÓN ĂN TỪ TABLEBOOKING SANG ORDER
            if (booking.OrderItems != null && booking.OrderItems.Any())
            {
                foreach (var bookingItem in booking.OrderItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        MenuItemId = bookingItem.MenuItemId,
                        Quantity = bookingItem.Quantity,
                        Price = bookingItem.Price,
                        TableBookingId = null  // Xóa liên kết với TableBooking
                    };
                    
                    _context.OrderItems.Add(orderItem);
                }
                
                // XÓA CÁC ORDERITEMS CŨ KHỎI TABLEBOOKING
                _context.OrderItems.RemoveRange(booking.OrderItems);
                await _context.SaveChangesAsync();
            }
            
            // XÓA BOOKING SAU KHI ĐÃ CHUYỂN SANG ORDER
            _context.TableBookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Json(new { 
                success = true, 
                message = "Đã xác nhận đặt bàn và tạo đơn hàng thành công!",
                orderId = order.Id 
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
                    return Json(new { 
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
                    return Json(new { 
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

                return Json(new { 
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

    public class WalkInBookingRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int Guests { get; set; }
        public int TableId { get; set; }
        public string? Note { get; set; }
    }
}