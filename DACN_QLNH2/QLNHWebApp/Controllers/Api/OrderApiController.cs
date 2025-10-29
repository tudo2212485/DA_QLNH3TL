using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;
using QLNHWebApp.Helpers;
using QLNHWebApp.Controllers;

namespace QLNHWebApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderApiController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public OrderApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        [HttpPost("save-booking-info")]
        public IActionResult SaveBookingInfo([FromBody] BookingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Save booking info to session
            var bookingInfo = new BookingInfo
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Date = request.Date.ToString("yyyy-MM-dd"),
                Time = request.Time,
                Guests = request.Guests,
                Note = request.Note ?? ""
            };

            HttpContext.Session.SetObjectAsJson("BookingInfo", bookingInfo);

            return Ok(new { message = "Booking info saved successfully" });
        }

        [HttpPost("booking")]
        public async Task<ActionResult<Order>> CreateBooking([FromBody] BookingRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = new Order
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Date = request.Date,
                Time = request.Time,
                Guests = request.Guests,
                Note = request.Note,
                TotalPrice = 0, // Booking only, no food items
                Status = "Chờ xác nhận",
                OrderItems = new List<OrderItem>()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CheckoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Calculate total price
            var menuItems = await _context.MenuItems
                .Where(m => request.Items.Select(i => i.MenuItemId).Contains(m.Id))
                .ToListAsync();

            decimal totalPrice = request.Items.Sum(item => 
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                return menuItem?.Price * item.Quantity ?? 0;
            });

            var order = new Order
            {
                CustomerName = request.CustomerName,
                Phone = request.Phone,
                Date = request.Date,
                Time = request.Time,
                Guests = request.Guests,
                Note = request.Note,
                TotalPrice = totalPrice,
                Status = request.PaymentMethod,
                TableId = request.TableId,
                OrderItems = new List<OrderItem>()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add order items
            foreach (var item in request.Items)
            {
                var menuItem = menuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                if (menuItem != null)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = menuItem.Price
                    });
                }
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return order;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Table)
                .ToListAsync();
        }
    }

    public class BookingRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int Guests { get; set; }
        public string? Note { get; set; }
    }

    public class CheckoutRequest
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int Guests { get; set; }
        public string? Note { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public int? TableId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}

