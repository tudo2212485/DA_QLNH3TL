using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    /// <summary>
    /// Controller xử lý chọn bàn (Table Selection) cho khách hàng
    /// Flow: Chọn tầng dựa vào số khách → Chọn bàn cụ thể → Xem chi tiết bàn
    /// Logic: Số khách định tầng nào có thể chọn, sau đó lọc bàn trống
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] // MVC Controller
    public class TableController : Controller
    {
        // Database context để query Tables và TableBookings
        private readonly RestaurantDbContext _context;

        // Constructor: Dependency Injection
        public TableController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Hiển thị các tầng có thể chọn dựa vào số khách (GET)
        /// Route: /Table/SelectFloor?guests=5
        /// Logic: 
        /// - 1-4 người: Tầng 1, 2, Sân thượng (bàn nhỏ)
        /// - 5-8 người: Tầng 1, 2 (bàn trung bình)
        /// - 9-15 người: Tầng 1, 2 (bàn lớn)
        /// - 16-20 người: Chỉ Tầng 1 (bàn VIP)
        /// </summary>
        /// <param name="guests">Số khách</param>
        /// <returns>View SelectFloor.cshtml với danh sách tầng</returns>
        public IActionResult SelectFloor(int guests)
        {
            // Danh sách tầng có thể chọn
            var availableFloors = new List<string>();

            // Logic phân tầng theo số khách
            if (guests <= 4)
            {
                // Nhóm nhỏ: Có thể ngồi mọi tầng
                availableFloors.AddRange(new[] { "Tầng 1", "Tầng 2", "Sân thượng" });
            }
            else if (guests <= 8)
            {
                // Nhóm trung bình: Tầng 1 và 2 (bàn 6-8 người)
                availableFloors.AddRange(new[] { "Tầng 1", "Tầng 2" });
            }
            else if (guests <= 15)
            {
                // Nhóm lớn: Tầng 1 và 2 (bàn 10-15 người)
                availableFloors.AddRange(new[] { "Tầng 1", "Tầng 2" });
            }
            else if (guests <= 20)
            {
                // Nhóm rất lớn: Chỉ Tầng 1 (bàn VIP 20 người)
                availableFloors.Add("Tầng 1");
            }

            // Truyền data sang View qua ViewBag
            ViewBag.Guests = guests;
            ViewBag.AvailableFloors = availableFloors;

            return View(); // Render SelectFloor.cshtml
        }

        /// <summary>
        /// Hiển thị danh sách bàn theo tầng đã chọn (GET)
        /// Route: /Table/SelectTable?floor=Tầng 1&guests=5&bookingDate=2024-12-05&bookingTime=19:00
        /// Logic: Lọc bàn theo tầng + sức chứa + trống tại thời điểm đặt
        /// </summary>
        /// <param name="floor">Tầng đã chọn (Tầng 1, Tầng 2, Sân thượng)</param>
        /// <param name="guests">Số khách</param>
        /// <param name="bookingDate">Ngày đặt (mặc định hôm nay)</param>
        /// <param name="bookingTime">Giờ đặt (mặc định 18:00)</param>
        /// <returns>View SelectTable.cshtml với danh sách bàn và trạng thái</returns>
        public async Task<IActionResult> SelectTable(string floor, int guests, DateTime? bookingDate = null, string? bookingTime = null)
        {
            // BƯỚC 1: Set giá trị mặc định nếu không truyền
            if (bookingDate == null) bookingDate = DateTime.Today;  // Hôm nay
            if (string.IsNullOrEmpty(bookingTime)) bookingTime = "18:00"; // 6h chiều

            // BƯỚC 2: Lấy danh sách bàn phù hợp từ database
            var tables = await _context.Tables
                .Where(t => t.Floor == floor           // Bàn thuộc tầng đã chọn
                         && t.Capacity >= guests       // Sức chứa đủ số khách
                         && t.IsActive)                // Bàn đang hoạt động (không bị khóa)
                .OrderBy(t => t.Name)                  // Sắp xếp theo tên bàn
                .ToListAsync();

            // BƯỚC 3: Kiểm tra bàn nào đã được đặt tại thời điểm đó
            // Lấy danh sách TableId của các booking chưa hủy
            var bookedTableIds = await _context.TableBookings
                .Where(tb => tb.BookingDate.Date == bookingDate.Value.Date // Cùng ngày
                           && tb.BookingTime == bookingTime                // Cùng giờ
                           && tb.Status != "Cancelled")                    // Chưa hủy
                .Select(tb => tb.TableId)              // Chỉ lấy TableId
                .ToListAsync();

            // BƯỚC 4: Truyền data sang View qua ViewBag
            ViewBag.Floor = floor;
            ViewBag.Guests = guests;
            ViewBag.BookingDate = bookingDate;
            ViewBag.BookingTime = bookingTime;
            ViewBag.BookedTableIds = bookedTableIds; // View sẽ dùng để hiển thị "Hết chỗ"

            // Trả về View với model = danh sách bàn
            return View(tables); // Render SelectTable.cshtml
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

