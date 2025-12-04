using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;

namespace QLNHWebApp.Controllers
{
    /// <summary>
    /// Controller xử lý thanh toán (Payment Flow)
    /// Hỗ trợ 2 loại: Order thường và TableBooking (đặt bàn trước)
    /// Flow: BookingController tạo Order → Session lưu ID → PaymentController lấy → Hiển thị trang thanh toán
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // MVC Controller, không hiển thị trong Swagger
    public class PaymentController : Controller
    {
        // Database context để query Order và TableBooking
        private readonly RestaurantDbContext _context;

        // Constructor: Dependency Injection
        public PaymentController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hiển thị trang thanh toán (GET)
        /// Route: /Payment hoặc /Payment/Index?orderId=123 hoặc /Payment/Index?bookingId=456
        /// Hỗ trợ 2 loại: Order thường (từ giỏ hàng) hoặc TableBooking (đặt bàn trước)
        /// </summary>
        /// <param name="orderId">ID đơn hàng thường (optional)</param>
        /// <param name="bookingId">ID đặt bàn (optional)</param>
        /// <returns>View thanh toán với thông tin Order hoặc Booking</returns>
        public async Task<IActionResult> Index(int? orderId = null, int? bookingId = null)
        {
            try
            {
                Console.WriteLine("PaymentController.Index called"); // Debug log

                // BƯỚC 1: Kiểm tra BookingId (ưu tiên cao hơn vì có thể có món ăn kèm theo)
                // Lấy từ parameter URL hoặc Session (BookingController đã lưu)
                // ?? operator: nếu bookingId null thì lấy từ Session
                var currentBookingId = bookingId ?? HttpContext.Session.GetInt32("CurrentBookingId");
                Console.WriteLine($"CurrentBookingId: {currentBookingId}");

                // Nếu có bookingId, load thông tin từ bảng TableBookings
                if (currentBookingId != null)
                {
                    // Query database với Include để load related data (Eager Loading)
                    var tableBooking = await _context.TableBookings
                        .Include(tb => tb.Table)        // Load thông tin bàn
                        .Include(tb => tb.OrderItems)   // Load các món đã chọn
                        .ThenInclude(oi => oi.MenuItem) // Load chi tiết món (tên, giá...)
                        .FirstOrDefaultAsync(tb => tb.Id == currentBookingId);

                    if (tableBooking != null)
                    {
                        Console.WriteLine($"Found booking: {tableBooking.Id}");

                        // Đánh dấu đây là booking (dùng trong View để hiển thị khác)
                        ViewBag.IsTableBooking = true;

                        // Trả về View riêng cho Booking (Index_Booking.cshtml)
                        return View("Index_Booking", tableBooking);
                    }
                }

                // BƯỚC 2: Nếu không có bookingId, thử tìm orderId (Order thường)
                // Lấy từ parameter URL hoặc Session (BookingController đã lưu)
                var currentOrderId = orderId ?? HttpContext.Session.GetInt32("CurrentOrderId");
                Console.WriteLine($"CurrentOrderId: {currentOrderId}");

                // Nếu không có cả orderId lẫn bookingId → Lỗi
                if (currentOrderId == null)
                {
                    Console.WriteLine("No order ID or booking ID found in session or parameter");

                    // TempData: Lưu message hiển thị 1 lần (survive 1 redirect)
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt bàn. Vui lòng đặt bàn lại.";

                    // Redirect về trang đặt bàn để user làm lại
                    return RedirectToAction("Table", "Booking");
                }

                // BƯỚC 3: Lấy thông tin Order từ database
                var order = await _context.Orders
                    .Include(o => o.OrderItems)      // Load các món trong đơn
                    .ThenInclude(oi => oi.MenuItem)  // Load chi tiết món
                    .FirstOrDefaultAsync(o => o.Id == currentOrderId);

                // Nếu không tìm thấy Order → Lỗi (có thể đã bị xóa hoặc ID sai)
                if (order == null)
                {
                    Console.WriteLine($"Order {currentOrderId} not found");
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng. Vui lòng đặt bàn lại.";
                    return RedirectToAction("Table", "Booking");
                }

                // BƯỚC 4: Trả về View thanh toán cho Order thường
                Console.WriteLine($"Returning view for order {order.Id}");
                ViewBag.IsTableBooking = false; // Đánh dấu không phải booking
                return View("Index_Simple", order); // View đơn giản hơn
            }
            catch (Exception ex)
            {
                // Xử lý lỗi: Log và trả về error message
                Console.WriteLine($"Error in PaymentController.Index: {ex.Message}");
                return Content($"Error: {ex.Message}"); // Hiển thị lỗi trên browser
            }
        }

        /// <summary>
        /// Xử lý thanh toán cho Order thường (POST)
        /// Route: POST /Payment/ProcessPayment
        /// Flow: Lấy Order → Cập nhật Status → Xóa Session → Redirect Success
        /// </summary>
        /// <param name="orderId">ID đơn hàng</param>
        /// <param name="paymentMethod">Phương thức thanh toán (restaurant/ewallet/bank_transfer)</param>
        /// <returns>Redirect đến trang Success</returns>
        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int orderId, string paymentMethod)
        {
            // BƯỚC 1: Lấy Order từ database
            var order = await _context.Orders
                .Include(o => o.OrderItems)     // Load món ăn
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)          // Load thông tin bàn (nếu có)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("Index");
            }

            // BƯỚC 2: Cập nhật trạng thái Order
            order.Status = "Đã thanh toán"; // Pending → Đã thanh toán
            await _context.SaveChangesAsync(); // Execute SQL UPDATE

            // BƯỚC 3: Xóa orderId khỏi session (không cần nữa)
            HttpContext.Session.Remove("CurrentOrderId");

            // BƯỚC 4: Hiển thị thông báo thành công (TempData survive 1 redirect)
            TempData["SuccessMessage"] = $"Thanh toán thành công! Mã đơn hàng: {order.Id}. Phương thức: {GetPaymentMethodName(paymentMethod)}";

            // BƯỚC 5: Redirect sang trang Success với orderId
            return RedirectToAction("Success", new { orderId = order.Id });
        }

        /// <summary>
        /// Xử lý thanh toán cho TableBooking (đặt bàn trước) - POST
        /// Route: POST /Payment/ProcessBookingPayment
        /// LOGIC MỚI: BẮT BUỘC thanh toán cọc 100,000đ dù chọn phương thức nào
        /// </summary>
        /// <param name="bookingId">ID đặt bàn</param>
        /// <param name="paymentMethod">Phương thức thanh toán (cod_deposit/ewallet/bank_transfer)</param>
        /// <returns>Redirect đến trang thanh toán online hoặc BookingSuccess</returns>
        [HttpPost]
        public async Task<IActionResult> ProcessBookingPayment(int bookingId, string paymentMethod)
        {
            // BƯỚC 1: Lấy Booking từ database
            var booking = await _context.TableBookings
                .Include(tb => tb.Table)        // Load thông tin bàn
                .Include(tb => tb.OrderItems)   // Load món ăn đã chọn (nếu có)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == bookingId);

            if (booking == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đặt bàn.";
                return RedirectToAction("Index");
            }

            // BƯỚC 2: Tính tổng tiền cần thanh toán
            const decimal DEPOSIT_FEE = 100000; // Phí cọc bắt buộc
            decimal totalFoodAmount = booking.OrderItems?.Sum(oi => oi.Price * oi.Quantity) ?? 0;
            decimal paymentAmount = 0;

            // Xác định số tiền cần thanh toán dựa vào phương thức
            if (paymentMethod == "cod_deposit")
            {
                // COD: Chỉ thanh toán cọc 100k ngay, số còn lại trả tại quầy
                paymentAmount = DEPOSIT_FEE;
            }
            else if (paymentMethod == "ewallet" || paymentMethod == "bank_transfer")
            {
                // Ví điện tử/Chuyển khoản: Thanh toán FULL (cọc + món ăn)
                paymentAmount = DEPOSIT_FEE + totalFoodAmount;
            }
            else
            {
                TempData["ErrorMessage"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("Index", new { bookingId = bookingId });
            }

            // BƯỚC 3: Xử lý thanh toán theo phương thức
            if (paymentMethod == "cod_deposit")
            {
                // TODO: Tích hợp VNPay để thanh toán cọc 100k
                // Tạm thời giả lập thanh toán thành công
                booking.Status = "Pending"; // Giữ nguyên status, chờ admin xác nhận

                // Lưu thông tin thanh toán (có thể tạo bảng Payment riêng)
                // _context.Payments.Add(new Payment { 
                //     BookingId = bookingId, 
                //     Amount = DEPOSIT_FEE, 
                //     Method = "COD_DEPOSIT",
                //     Status = "Paid" 
                // });

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Thanh toán cọc 100,000đ thành công! Mã đặt bàn: #{bookingId}. Số tiền còn lại {totalFoodAmount:N0}đ sẽ thanh toán tại quầy.";
            }
            else
            {
                // Ví điện tử / Chuyển khoản: Redirect đến VNPay để thanh toán full
                // TODO: Tích hợp VNPay API
                TempData["SuccessMessage"] = $"Đang chuyển đến trang thanh toán {paymentAmount:N0}đ...";

                // Giả lập thanh toán thành công
                booking.Status = "Confirmed"; // Đã thanh toán full → tự động confirm
                await _context.SaveChangesAsync();
            }

            // BƯỚC 4: Xóa bookingId khỏi session
            HttpContext.Session.Remove("CurrentBookingId");

            // BƯỚC 5: Redirect sang trang BookingSuccess
            return RedirectToAction("BookingSuccess", new { bookingId = booking.Id });
        }

        /// <summary>
        /// Trang thành công cho Đặt Bàn (GET)
        /// Route: /Payment/BookingSuccess?bookingId=123
        /// Hiển thị thông tin booking vừa tạo, hướng dẫn khách chờ xác nhận
        /// </summary>
        /// <param name="bookingId">ID đặt bàn</param>
        /// <returns>View BookingSuccess.cshtml với model TableBooking</returns>
        public async Task<IActionResult> BookingSuccess(int bookingId)
        {
            // Lấy thông tin booking đầy đủ từ database
            var booking = await _context.TableBookings
                .Include(tb => tb.Table)        // Bàn số mấy
                .Include(tb => tb.OrderItems)   // Món đã chọn (nếu có)
                .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(tb => tb.Id == bookingId);

            // Nếu không tìm thấy (ID sai hoặc đã xóa) → Về trang chủ
            if (booking == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Trả về View BookingSuccess.cshtml với model TableBooking
            return View(booking);
        }

        /// <summary>
        /// Trang thành công cho Order thường (GET)
        /// Route: /Payment/Success?orderId=456
        /// Hiển thị thông tin đơn hàng vửa thanh toán, cảm ơn khách
        /// </summary>
        /// <param name="orderId">ID đơn hàng</param>
        /// <returns>View Success.cshtml với model Order</returns>
        public async Task<IActionResult> Success(int orderId)
        {
            // Lấy thông tin order đầy đủ từ database
            var order = await _context.Orders
                .Include(o => o.OrderItems)     // Các món trong đơn
                .ThenInclude(oi => oi.MenuItem) // Chi tiết món
                .FirstOrDefaultAsync(o => o.Id == orderId);

            // Nếu không tìm thấy → Về trang chủ
            if (order == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Trả về View Success.cshtml với model Order
            return View(order);
        }

        /// <summary>
        /// Helper method: Chuyển đổi payment method code thành tiếng Việt
        /// </summary>
        /// <param name="method">Mã phương thức (cod_deposit/ewallet/bank_transfer)</param>
        /// <returns>Tên phương thức tiếng Việt</returns>
        private string GetPaymentMethodName(string method)
        {
            // Switch expression (C# 8.0+) - ngắn gọn hơn switch statement
            return method switch
            {
                "cod_deposit" => "Đặt cọc giữ bàn (COD)",   // Cọc 100k ngay, còn lại trả sau
                "restaurant" => "Thanh toán tại nhà hàng",  // Legacy - backward compatibility
                "ewallet" => "Ví điện tử",                 // MoMo, ZaloPay, VNPay...
                "bank_transfer" => "Chuyển khoản ngân hàng", // Chuyển khoản trực tiếp
                _ => "Không xác định"                     // Default case (nếu không khớp case nào)
            };
        }
    }
}