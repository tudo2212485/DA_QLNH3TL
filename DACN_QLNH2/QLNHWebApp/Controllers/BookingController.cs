using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;

namespace QLNHWebApp.Controllers
{
    /// <summary>
    /// Controller xử lý đặt bàn từ phía khách hàng (Customer-facing)
    /// Flow: Khách chọn món → Nhập thông tin → Đặt bàn → Thanh toán
    /// Sử dụng Session để lưu tạm thông tin giữa các bước
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // Không hiển thị trong Swagger
    public class BookingController : Controller
    {
        // Database context để tạo Order và lưu vào CSDL
        private readonly RestaurantDbContext _context;

        // Constructor: Dependency Injection
        public BookingController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hiển thị trang đặt bàn (GET)
        /// Route: /Booking/Table
        /// Load thông tin booking từ Session nếu có (để giữ dữ liệu khi back)
        /// </summary>
        public IActionResult Table()
        {
            // Lấy thông tin booking từ Session (nếu user đã nhập trước đó)
            // Session giúp giữ dữ liệu giữa các request (server-side storage)
            var bookingInfo = HttpContext.Session.GetObjectFromJson<BookingInfo>("BookingInfo");
            
            // Truyền sang View qua ViewBag để hiển thị lại form
            ViewBag.BookingInfo = bookingInfo;
            
            return View(); // Render view Booking/Table.cshtml
        }

        /// <summary>
        /// Lưu thông tin đặt bàn vào Session (bước trung gian)
        /// Route: POST /Booking/SaveBookingInfo
        /// Dùng khi user muốn lưu thông tin rồi tiếp tục chọn món
        /// </summary>
        [HttpPost]
        public IActionResult SaveBookingInfo(string customerName, string phone, string date, string time, int guests, string note, string action)
        {
            // Tạo object BookingInfo từ form data
            var bookingInfo = new BookingInfo
            {
                CustomerName = customerName ?? "", // ?? "": nếu null thì gán rỗng
                Phone = phone ?? "",
                Date = date ?? "",
                Time = time ?? "",
                Guests = guests,
                Note = note ?? ""
            };

            // Lưu vào Session (server-side storage, giữ trong 20-30 phút)
            // SetObjectAsJson: Extension method tự viết để serialize object thành JSON
            HttpContext.Session.SetObjectAsJson("BookingInfo", bookingInfo);

            // Kiểm tra action: user muốn làm gì tiếp theo?
            if (action == "continue_menu")
            {
                // Nếu action = "continue_menu" → redirect về menu để chọn món
                return RedirectToAction("Index", "Home"); // Trang menu
            }

            // Mặc định: quay lại trang đặt bàn
            return RedirectToAction("Table");
        }

        /// <summary>
        /// Xử lý form submit đặt bàn (POST) - Action chính tạo Order
        /// Route: POST /Booking/Table
        /// Flow: Validate → Lấy Cart → Tính tiền → Tạo Order → Redirect Payment
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Table(string customerName, string phone, string date, string time, int guests, string note)
        {
            // BƯỚC 1: Lưu thông tin vào Session trước (để giữ dữ liệu nếu có lỗi)
            var bookingInfo = new BookingInfo
            {
                CustomerName = customerName ?? "",
                Phone = phone ?? "",
                Date = date ?? "",
                Time = time ?? "",
                Guests = guests,
                Note = note ?? ""
            };
            HttpContext.Session.SetObjectAsJson("BookingInfo", bookingInfo);

            try
            {
                // BƯỚC 2: VALIDATION - Kiểm tra dữ liệu đầu vào
                // Các trường bắt buộc phải có
                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(phone) ||
                    string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
                {
                    TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                    return View(bookingInfo); // Trả lại form với thông báo lỗi
                }

                // Validate định dạng ngày
                if (!DateTime.TryParse(date, out DateTime bookingDate))
                {
                    TempData["ErrorMessage"] = "Ngày đặt không hợp lệ.";
                    return View(bookingInfo);
                }

                // BƯỚC 3: Lấy giỏ hàng từ Session
                // Cart chứa các món đã chọn (nếu có)
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // BƯỚC 4: Tính tổng tiền và tạo OrderItems
                decimal totalPrice = 0;
                var orderItems = new List<OrderItem>();

                // Duyệt qua từng món trong giỏ hàng
                foreach (var cartItem in cart)
                {
                    // Lấy thông tin món từ database
                    var menuItem = await _context.MenuItems.FindAsync(cartItem.MenuItemId);
                    if (menuItem != null)
                    {
                        // Cộng dồn tổng tiền
                        totalPrice += menuItem.Price * cartItem.Quantity;
                        
                        // Tạo OrderItem (chi tiết món trong đơn)
                        orderItems.Add(new OrderItem
                        {
                            MenuItemId = cartItem.MenuItemId,
                            Quantity = cartItem.Quantity,
                            Price = menuItem.Price // Lưu giá tại thời điểm đặt
                        });
                    }
                }

                // BƯỚC 5: TẠO ORDER (đơn hàng chính)
                var order = new Order
                {
                    CustomerName = customerName,
                    Phone = phone,
                    Date = bookingDate,
                    Time = time,
                    Guests = guests,
                    Note = note ?? "",
                    TotalPrice = totalPrice, // Tổng tiền đã tính
                    Status = "Pending", // Trạng thái: Đang chờ xử lý
                    OrderItems = orderItems // Danh sách món
                };

                // Lưu vào database
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Execute SQL INSERT

                // Log để debug
                Console.WriteLine($"✅ Order created successfully! Order ID: {order.Id}");

                // BƯỚC 6: Lưu Order ID vào Session (dùng cho trang Payment)
                HttpContext.Session.SetInt32("CurrentOrderId", order.Id);

                // Thông báo thành công (hiển thị ở trang tiếp theo)
                TempData["SuccessMessage"] = $"Đặt bàn thành công! Mã đơn: #{order.Id}";

                // BƯỚC 7: Xóa Session sau khi tạo Order thành công
                HttpContext.Session.Remove("Cart"); // Xóa giỏ hàng
                HttpContext.Session.Remove("BookingInfo"); // Xóa thông tin booking

                // BƯỚC 8: Redirect sang trang thanh toán
                Console.WriteLine($"🔄 Redirecting to Payment page...");
                return RedirectToAction("Index", "Payment");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi: Log và hiển thị thông báo
                Console.WriteLine($"❌ Booking error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi đặt bàn: {ex.Message}";
                return View(bookingInfo); // Trả lại form với thông báo lỗi
            }
        }
    }

    #region Helper Classes
    
    /// <summary>
    /// Class đại diện cho 1 món trong giỏ hàng (lưu trong Session)
    /// Chỉ cần MenuItemId và Quantity, chi tiết món sẽ query từ DB
    /// </summary>
    public class CartItem
    {
        public int MenuItemId { get; set; } // ID món ăn
        public int Quantity { get; set; } // Số lượng
    }

    /// <summary>
    /// Class chứa thông tin đặt bàn tạm thời (lưu trong Session)
    /// Dùng để giữ dữ liệu khi user điền form, chọn món, rồi quay lại
    /// </summary>
    public class BookingInfo
    {
        public string CustomerName { get; set; } = ""; // Tên khách
        public string Phone { get; set; } = ""; // Số điện thoại
        public string Date { get; set; } = ""; // Ngày đặt (string format)
        public string Time { get; set; } = ""; // Giờ đặt (HH:mm)
        public int Guests { get; set; } = 1; // Số khách (mặc định 1)
        public string Note { get; set; } = ""; // Ghi chú
    }
    
    #endregion
}