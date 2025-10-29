using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;

namespace QLNHWebApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly RestaurantDbContext _context;

        public PaymentController(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? orderId = null)
        {
            try 
            {
                Console.WriteLine("PaymentController.Index called");
                
                // Lấy orderId từ session hoặc parameter
                var currentOrderId = orderId ?? HttpContext.Session.GetInt32("CurrentOrderId");
                Console.WriteLine($"CurrentOrderId: {currentOrderId}");
                
                if (currentOrderId == null)
                {
                    Console.WriteLine("No order ID found, getting latest order");
                    
                    // Thử lấy order mới nhất nếu không có session
                    var latestOrder = await _context.Orders
                        .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.MenuItem)
                        .OrderByDescending(o => o.Id)
                        .FirstOrDefaultAsync();
                        
                    if (latestOrder != null)
                    {
                        Console.WriteLine($"Found latest order: {latestOrder.Id}");
                        return View(latestOrder);
                    }
                    
                    Console.WriteLine("No orders found, redirecting to booking");
                    TempData["ErrorMessage"] = "Không tìm thấy đơn hàng. Vui lòng đặt bàn lại.";
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