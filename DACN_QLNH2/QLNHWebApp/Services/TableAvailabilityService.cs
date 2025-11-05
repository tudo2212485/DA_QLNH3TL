using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Services
{
    public class TableAvailabilityService
    {
        private readonly RestaurantDbContext _context;

        public TableAvailabilityService(RestaurantDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách bàn có sẵn theo tầng, số khách, ngày và giờ
        public async Task<List<Table>> GetAvailableTablesAsync(string floor, int guests, DateTime bookingDate, string bookingTime)
        {
            // Lấy tất cả bàn phù hợp về sức chứa
            var allTables = await _context.Tables
                .Where(t => t.Floor == floor && t.Capacity >= guests && t.IsActive)
                .ToListAsync();

            // Lấy danh sách bàn đã được đặt
            var bookedTableIds = await GetBookedTableIdsAsync(bookingDate, bookingTime);

            // Trả về bàn chưa được đặt
            return allTables.Where(t => !bookedTableIds.Contains(t.Id)).ToList();
        }

        // Lấy danh sách ID các bàn đã được đặt
        public async Task<List<int>> GetBookedTableIdsAsync(DateTime bookingDate, string bookingTime)
        {
            return await _context.TableBookings
                .Where(tb => tb.BookingDate.Date == bookingDate.Date
                           && tb.BookingTime == bookingTime
                           && tb.Status != "Cancelled")
                .Select(tb => tb.TableId)
                .ToListAsync();
        }

        // Kiểm tra bàn có trống không
        public async Task<bool> IsTableAvailableAsync(int tableId, DateTime bookingDate, string bookingTime, int? excludeBookingId = null)
        {
            var query = _context.TableBookings
                .Where(tb => tb.TableId == tableId
                           && tb.BookingDate.Date == bookingDate.Date
                           && tb.BookingTime == bookingTime
                           && tb.Status != "Cancelled");

            // Loại trừ booking hiện tại khi edit
            if (excludeBookingId.HasValue)
            {
                query = query.Where(tb => tb.Id != excludeBookingId.Value);
            }

            return !await query.AnyAsync();
        }

        // Lấy trạng thái bàn
        public async Task<string> GetTableStatusAsync(int tableId, DateTime bookingDate, string bookingTime)
        {
            var isAvailable = await IsTableAvailableAsync(tableId, bookingDate, bookingTime);
            return isAvailable ? "Available" : "Booked";
        }
    }
}
