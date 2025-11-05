using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;

namespace QLNHWebApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly RestaurantDbContext _context;

        public BookingController(RestaurantDbContext context)
        {
            _context = context;
        }

        public IActionResult Table()
        {
            // Load booking info from session if exists
            var bookingInfo = HttpContext.Session.GetObjectFromJson<BookingInfo>("BookingInfo");
            ViewBag.BookingInfo = bookingInfo;
            return View();
        }

        [HttpPost]
        public IActionResult SaveBookingInfo(string customerName, string phone, string date, string time, int guests, string note, string action)
        {
            // Save booking info to session
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
            
            if (action == "continue_menu")
            {
                // Redirect to menu page to select dishes
                return RedirectToAction("Index", "Home"); // hoặc Menu controller nếu có
            }
            
            return RedirectToAction("Table");
        }

        [HttpPost]
        public async Task<IActionResult> Table(string customerName, string phone, string date, string time, int guests, string note)
        {
            // Save booking info to session first
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
                // Validate input
                if (string.IsNullOrEmpty(customerName) || string.IsNullOrEmpty(phone) || 
                    string.IsNullOrEmpty(date) || string.IsNullOrEmpty(time))
                {
                    TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin bắt buộc.";
                    return View(bookingInfo);
                }

                // Validate date format
                if (!DateTime.TryParse(date, out DateTime bookingDate))
                {
                    TempData["ErrorMessage"] = "Ngày đặt không hợp lệ.";
                    return View(bookingInfo);
                }

                // Get cart items from session (optional)
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
                
                // Calculate total price
                decimal totalPrice = 0;
                var orderItems = new List<OrderItem>();

                // Only process cart items if cart is not empty
                foreach (var cartItem in cart)
                {
                    var menuItem = await _context.MenuItems.FindAsync(cartItem.MenuItemId);
                    if (menuItem != null)
                    {
                        totalPrice += menuItem.Price * cartItem.Quantity;
                        orderItems.Add(new OrderItem
                        {
                            MenuItemId = cartItem.MenuItemId,
                            Quantity = cartItem.Quantity,
                            Price = menuItem.Price
                        });
                    }
                }

                // Create order
                var order = new Order
                {
                    CustomerName = customerName,
                    Phone = phone,
                    Date = bookingDate,
                    Time = time,
                    Guests = guests,
                    Note = note ?? "",
                    TotalPrice = totalPrice,
                    Status = "Pending",
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Order created successfully! Order ID: {order.Id}");

                // Store order ID in session for payment
                HttpContext.Session.SetInt32("CurrentOrderId", order.Id);
                
                // Store success message
                TempData["SuccessMessage"] = $"Đặt bàn thành công! Mã đơn: #{order.Id}";

                // Clear cart after successful booking
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("BookingInfo");

                // ALWAYS redirect to payment page after successful booking
                Console.WriteLine($"🔄 Redirecting to Payment page...");
                return RedirectToAction("Index", "Payment");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"❌ Booking error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi đặt bàn: {ex.Message}";
                return View(bookingInfo);
            }
        }
    }

    public class CartItem
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }

    public class BookingInfo
    {
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Date { get; set; } = "";
        public string Time { get; set; } = "";
        public int Guests { get; set; } = 1;
        public string Note { get; set; } = "";
    }
}