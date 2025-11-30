using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;

namespace QLNHWebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController : Controller
    {
        private readonly RestaurantDbContext _context;

        public PaymentController(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? orderId = null, int? bookingId = null)
        {
            try
            {
                Console.WriteLine("PaymentController.Index called");

                // Ưu tiên bookingId từ parameter hoặc session
                var currentBookingId = bookingId ?? HttpContext.Session.GetInt32("CurrentBookingId");
                Console.WriteLine($"CurrentBookingId: {currentBookingId}");

                // Nếu có bookingId, load thông tin từ TableBooking
                if (currentBookingId != null)
                {
                    var tableBooking = await _context.TableBookings
                        .Include(tb => tb.Table)
                        .Include(tb => tb.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                        .FirstOrDefaultAsync(tb => tb.Id == currentBookingId);

                    if (tableBooking != null)
                    {
                        Console.WriteLine($"Found booking: {tableBooking.Id}");
                        ViewBag.IsTableBooking = true;
                        return View("Index_Booking", tableBooking);
                    }
                }

                // Nếu không có bookingId, thử tìm orderId
                var currentOrderId = orderId ?? HttpContext.Session.GetInt32("CurrentOrderId");
                Console.WriteLine($"CurrentOrderId: {currentOrderId}");

                if (currentOrderId == null)
                {
                    Console.WriteLine("No order ID or booking ID found in session or parameter");
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt bàn. Vui lòng đặt bàn lại.";
                    return RedirectToAction("Table", "Booking");
                }

                // Lấy thông tin order từ database
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                    .FirstOrDefaultAsync(o => o.Id == currentOrderId);

                if (order == null)
                {
                    Console.WriteLine($"Order {currentOrderId} not found");
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng. Vui lòng đặt bàn lại.";
                    return RedirectToAction("Table", "Booking");
                }

                Console.WriteLine($"Returning view for order {order.Id}");
                ViewBag.IsTableBooking = false;
                return View("Index_Simple", order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PaymentController.Index: {ex.Message}");
                return Content($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string paymentMethod)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

            // Cập nhật trạng thái đơn hàng
            order.Status = "Đã thanh toán";
            await _context.SaveChangesAsync();

            // Xóa orderId khỏi session
            HttpContext.Session.Remove("CurrentOrderId");

            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = $"Thanh toán thành công! Mã đơn hàng: {order.Id}. Phương thức: {GetPaymentMethodName(paymentMethod)}";

            return RedirectToAction("Success", new { orderId = order.Id });
        }

        [HttpPost]
        public async Task<IActionResult> ProcessBookingPayment(int bookingId, string paymentMethod)
        {
            var booking = await _context.TableBookings
                .Include(tb => tb.Table)
                .Include(tb => tb.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == bookingId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt bàn.";
                return RedirectToAction("Index");
            }

            // GIỮ NGUYÊN trạng thái "Pending" để admin xác nhận
            // Không thay đổi gì trong booking, chỉ lưu thông tin thanh toán (có thể lưu vào bảng khác nếu cần)

            // Xóa bookingId khỏi session
            HttpContext.Session.Remove("CurrentBookingId");

            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = $"Đặt bàn thành công! Mã đặt bàn: #{bookingId}. Phương thức thanh toán: {GetPaymentMethodName(paymentMethod)}. Đơn đặt bàn đang chờ xác nhận từ nhà hàng.";

            return RedirectToAction("BookingSuccess", new { bookingId = booking.Id });
        }

        public async Task<IActionResult> BookingSuccess(int bookingId)
        {
            var booking = await _context.TableBookings
                .Include(tb => tb.Table)
                .Include(tb => tb.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == bookingId);

            if (booking == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(booking);
        }

        public async Task<IActionResult> Success(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }

        private string GetPaymentMethodName(string method)
        {
            return method switch
            {
                "restaurant" => "Thanh toán tại nhà hàng",
                "ewallet" => "Ví điện tử",
                "bank_transfer" => "Chuyển khoản ngân hàng",
                _ => "Không xác định"
            };
        }
    }
}