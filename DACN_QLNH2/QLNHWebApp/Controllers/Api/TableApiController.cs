using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TableApiController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public TableApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        // GET: API - Lấy tất cả bàn để test
        [HttpGet("GetAllTables")]
        public async Task<IActionResult> GetAllTables()
        {
            try
            {
                var tables = await _context.Tables
                    .Where(t => t.IsActive)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Floor,
                        t.Capacity,
                        t.Description,
                        t.ImageUrl
                    })
                    .ToListAsync();

                return Ok(new { count = tables.Count, tables });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: API - Lấy danh sách bàn theo tầng và số khách
        [HttpGet("GetTablesByFloor")]
        public async Task<IActionResult> GetTablesByFloor(string floor, int guests)
        {
            try
            {
                // Debug: Log the parameters
                Console.WriteLine($"GetTablesByFloor called with floor: '{floor}', guests: {guests}");
                
                // Get all tables first to debug
                var allTables = await _context.Tables.Where(t => t.IsActive).ToListAsync();
                Console.WriteLine($"Total active tables: {allTables.Count}");
                
                foreach (var table in allTables.Take(3))
                {
                    Console.WriteLine($"Table: {table.Name}, Floor: '{table.Floor}', Capacity: {table.Capacity}");
                }

                var tables = await _context.Tables
                    .Where(t => t.Floor == floor && t.Capacity >= guests && t.IsActive)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Floor,
                        t.Capacity,
                        t.Description,
                        t.ImageUrl
                    })
                    .ToListAsync();

                Console.WriteLine($"Filtered tables count: {tables.Count}");
                return Ok(new { 
                    requestedFloor = floor, 
                    requestedGuests = guests,
                    totalTables = allTables.Count,
                    filteredTables = tables.Count,
                    tables = tables 
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTablesByFloor: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: API - Kiểm tra bàn có trống không
        [HttpGet("CheckTableAvailability")]
        public async Task<IActionResult> CheckTableAvailability(int tableId, DateTime bookingDate, string bookingTime)
        {
            try
            {
                var isAvailable = !await _context.TableBookings
                    .AnyAsync(tb => tb.TableId == tableId 
                                  && tb.BookingDate.Date == bookingDate.Date 
                                  && tb.BookingTime == bookingTime
                                  && tb.Status != "Cancelled");

                return Ok(new { isAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: API - Lấy danh sách bàn trống (cho walk-in booking)
        [HttpGet("GetAvailableTables")]
        public async Task<IActionResult> GetAvailableTables(int? guests = null)
        {
            try
            {
                // Lấy danh sách ID của các bàn đã được đặt (trong orders đang hoạt động)
                var occupiedTableIds = await _context.Orders
                    .Where(o => o.Status == "Đang phục vụ" || 
                               o.Status == "Chưa thanh toán" || 
                               o.Status == "Đã xác nhận")
                    .Select(o => o.TableId)
                    .Distinct()
                    .ToListAsync();

                // Lấy các bàn KHÔNG nằm trong danh sách đã đặt
                var query = _context.Tables
                    .Where(t => t.IsActive && !occupiedTableIds.Contains(t.Id));

                // Lọc theo số khách nếu có
                if (guests.HasValue && guests.Value > 0)
                {
                    query = query.Where(t => t.Capacity >= guests.Value);
                }

                var tables = await query
                    .OrderBy(t => t.Floor)
                    .ThenBy(t => t.Name)
                    .Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Floor,
                        t.Capacity,
                        t.Description
                    })
                    .ToListAsync();

                return Ok(tables);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: API - Đặt bàn
        [HttpPost("BookTable")]
        public async Task<IActionResult> BookTable([FromBody] TableBookingRequest request)
        {
            try
            {
                // DEBUG: Log incoming request
                Console.WriteLine($"📝 BookTable API called:");
                Console.WriteLine($"   - TableId: {request.TableId}");
                Console.WriteLine($"   - Customer: {request.CustomerName}");
                Console.WriteLine($"   - Phone: {request.Phone}");
                Console.WriteLine($"   - Date: {request.BookingDate}");
                Console.WriteLine($"   - Time: {request.BookingTime}");
                Console.WriteLine($"   - Guests: {request.Guests}");
                Console.WriteLine($"   - Items count: {request.OrderItems?.Count ?? 0}");
                
                // Kiểm tra bàn có tồn tại và còn trống không
                var table = await _context.Tables.FindAsync(request.TableId);
                if (table == null)
                {
                    return Ok(new { success = false, message = "Bàn không tồn tại!" });
                }

                // Kiểm tra bàn đã được đặt chưa
                var existingBooking = await _context.TableBookings
                    .FirstOrDefaultAsync(tb => tb.TableId == request.TableId 
                                             && tb.BookingDate.Date == request.BookingDate.Date 
                                             && tb.BookingTime == request.BookingTime
                                             && tb.Status != "Cancelled");

                if (existingBooking != null)
                {
                    return Ok(new { success = false, message = "Bàn đã được đặt trong thời gian này!" });
                }

                // Tạo booking mới
                var booking = new TableBooking
                {
                    TableId = request.TableId,
                    CustomerName = request.CustomerName,
                    Phone = request.Phone,
                    BookingDate = request.BookingDate,
                    BookingTime = request.BookingTime,
                    Guests = request.Guests,
                    Note = request.Note,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.TableBookings.Add(booking);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"✅ Booking saved! ID: {booking.Id}, Status: {booking.Status}");

                // Thêm món ăn vào booking nếu có
                if (request.OrderItems != null && request.OrderItems.Any())
                {
                    foreach (var item in request.OrderItems)
                    {
                        var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                        if (menuItem != null)
                        {
                            var orderItem = new OrderItem
                            {
                                TableBookingId = booking.Id,  // Liên kết với TableBooking
                                MenuItemId = item.MenuItemId,
                                Quantity = item.Quantity,
                                Price = menuItem.Price
                            };
                            _context.OrderItems.Add(orderItem);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "Đặt bàn thành công!", bookingId = booking.Id });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }

    public class TableBookingRequest
    {
        public int TableId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string BookingTime { get; set; } = string.Empty;
        public int Guests { get; set; }
        public string? Note { get; set; }
        public List<BookingOrderItem>? OrderItems { get; set; }
    }

    public class BookingOrderItem
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
