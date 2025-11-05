using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Services;
using QLNHWebApp.Helpers;

namespace QLNHWebApp.Controllers
{
    public class TableController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly TableAvailabilityService _tableService;

        public TableController(RestaurantDbContext context, TableAvailabilityService tableService)
        {
            _context = context;
            _tableService = tableService;
        }

        // GET: Chọn tầng dựa trên số khách
        public IActionResult SelectFloor(int guests)
        {
            var availableFloors = new List<string>();

            if (guests <= 4)
            {
                availableFloors.AddRange(new[] { "Tầng 1", "Tầng 2", "Sân thượng" });
            }
            else if (guests <= 8)
            {
                availableFloors.AddRange(new[] { "Tầng 1", "Tầng 2" });
            }
            else if (guests <= 15)
            {
                availableFloors.AddRange(new[] { "Tầng 1", "Tầng 2" });
            }
            else if (guests <= 20)
            {
                availableFloors.Add("Tầng 1");
            }

            ViewBag.Guests = guests;
            ViewBag.AvailableFloors = availableFloors;
            return View();
        }

        // GET: Hiển thị danh sách bàn theo tầng
        public async Task<IActionResult> SelectTable(string floor, int guests, DateTime? bookingDate = null, string? bookingTime = null)
        {
            if (bookingDate == null) bookingDate = DateTime.Today;
            if (string.IsNullOrEmpty(bookingTime)) bookingTime = DateTime.Now.Hour >= 18 ? "18:00" : "12:00";

            // Lấy bàn khả dụng qua service
            var availableTables = await _tableService.GetAvailableTablesAsync(floor, guests, bookingDate.Value, bookingTime);
            
            // Lấy tất cả bàn để hiển thị (kể cả đã đặt)
            var allTables = await _context.Tables
                .Where(t => t.Floor == floor && t.Capacity >= guests && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            // Lấy danh sách bàn đã được đặt trong khung giờ này
            var bookedTableIds = await _tableService.GetBookedTableIdsAsync(bookingDate.Value, bookingTime);

            ViewBag.Floor = floor;
            ViewBag.Guests = guests;
            ViewBag.BookingDate = bookingDate;
            ViewBag.BookingTime = bookingTime;
            ViewBag.BookedTableIds = bookedTableIds;
            ViewBag.AvailableTables = availableTables.Select(t => t.Id).ToList();

            return View(allTables);
        }

        // GET: Xem chi tiết bàn
        public async Task<IActionResult> Details(int id)
        {
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == id);

            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // POST: Đặt bàn
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> BookTable([FromBody] BookTableRequest request)
        {
            try
            {
                Console.WriteLine("========== BOOK TABLE REQUEST ==========");
                Console.WriteLine($"TableId: {request.TableId}");
                Console.WriteLine($"CustomerName: {request.CustomerName}");
                Console.WriteLine($"Phone: {request.Phone}");
                Console.WriteLine($"Date: {request.BookingDate}");
                Console.WriteLine($"Time: {request.BookingTime}");
                Console.WriteLine($"Guests: {request.Guests}");

                // Validate input
                if (request.TableId <= 0 || string.IsNullOrEmpty(request.CustomerName) || 
                    string.IsNullOrEmpty(request.Phone) || string.IsNullOrEmpty(request.BookingDate) || 
                    string.IsNullOrEmpty(request.BookingTime))
                {
                    Console.WriteLine("❌ VALIDATION FAILED");
                    return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin." });
                }

                // Parse date
                if (!DateTime.TryParse(request.BookingDate, out DateTime bookingDate))
                {
                    Console.WriteLine("❌ INVALID DATE FORMAT");
                    return Json(new { success = false, message = "Ngày đặt không hợp lệ." });
                }

                // Check if table is available
                var isAvailable = await _tableService.IsTableAvailableAsync(request.TableId, bookingDate, request.BookingTime);
                if (!isAvailable)
                {
                    Console.WriteLine("❌ TABLE NOT AVAILABLE");
                    return Json(new { success = false, message = "Bàn này đã được đặt trong khung giờ này. Vui lòng chọn bàn khác." });
                }

                // Get cart items from session (optional)
                var cart = HttpContext.Session.GetObjectFromJson<List<QLNHWebApp.Controllers.CartItem>>("Cart") ?? new List<QLNHWebApp.Controllers.CartItem>();
                
                // Calculate total price
                decimal totalPrice = 0;
                var orderItems = new List<OrderItem>();

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
                    CustomerName = request.CustomerName,
                    Phone = request.Phone,
                    Date = bookingDate,
                    Time = request.BookingTime,
                    Guests = request.Guests,
                    Note = request.Note ?? "",
                    TotalPrice = totalPrice,
                    Status = "Pending",
                    TableId = request.TableId,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"✅ ORDER CREATED - ID: {order.Id}");

                // Create table booking
                var tableBooking = new TableBooking
                {
                    TableId = request.TableId,
                    CustomerName = request.CustomerName,
                    Phone = request.Phone,
                    BookingDate = bookingDate,
                    BookingTime = request.BookingTime,
                    Guests = request.Guests,
                    Note = request.Note ?? "",
                    Status = "Pending"
                };

                _context.TableBookings.Add(tableBooking);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ TABLE BOOKING CREATED - ID: {tableBooking.Id}");

                // Store order ID in session for payment
                HttpContext.Session.SetInt32("CurrentOrderId", order.Id);
                Console.WriteLine($"✅ CurrentOrderId stored in session: {order.Id}");

                // Clear cart
                HttpContext.Session.Remove("Cart");

                return Json(new { 
                    success = true, 
                    message = "Đặt bàn thành công!",
                    orderId = order.Id,
                    redirectUrl = Url.Action("Index", "Payment")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌❌❌ BOOKING ERROR: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Có lỗi xảy ra: {ex.Message}" });
            }
        }
    }

    public class BookTableRequest
    {
        public int TableId { get; set; }
        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string BookingDate { get; set; } = "";
        public string BookingTime { get; set; } = "";
        public int Guests { get; set; }
        public string? Note { get; set; }
    }
}