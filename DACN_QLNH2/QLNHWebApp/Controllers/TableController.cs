using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class TableController : Controller
    {
        private readonly RestaurantDbContext _context;

        public TableController(RestaurantDbContext context)
        {
            _context = context;
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
            if (string.IsNullOrEmpty(bookingTime)) bookingTime = "18:00";

            var tables = await _context.Tables
                .Where(t => t.Floor == floor && t.Capacity >= guests && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            // Lấy danh sách bàn đã được đặt trong ngày và giờ đó
            var bookedTableIds = await _context.TableBookings
                .Where(tb => tb.BookingDate.Date == bookingDate.Value.Date
                           && tb.BookingTime == bookingTime
                           && tb.Status != "Cancelled")
                .Select(tb => tb.TableId)
                .ToListAsync();

            ViewBag.Floor = floor;
            ViewBag.Guests = guests;
            ViewBag.BookingDate = bookingDate;
            ViewBag.BookingTime = bookingTime;
            ViewBag.BookedTableIds = bookedTableIds;

            return View(tables);
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
    }
}

