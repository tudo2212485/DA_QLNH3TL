using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers.Api
{
    /// <summary>
    /// API Controller xử lý chatbot - Trả lời câu hỏi thường gặp của khách hàng
    /// Logic: Rule-based matching (phát hiện từ khóa trong câu hỏi)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChatBotApiController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public ChatBotApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Xử lý tin nhắn từ khách hàng
        /// Endpoint: POST /api/ChatBotApi/message
        /// </summary>
        [HttpPost("message")]
        public async Task<IActionResult> ProcessMessage([FromBody] ChatMessage request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(new { error = "Tin nhắn không được để trống" });
            }

            // Chuyển về chữ thường để so sánh
            var message = request.Message.ToLower().Trim();

            // BƯỚC 1: Kiểm tra intent (ý định) của câu hỏi
            var response = await DetectIntentAndRespond(message);

            return Ok(new
            {
                message = response.Message,
                suggestions = response.Suggestions,
                data = response.Data
            });
        }

        /// <summary>
        /// Phát hiện ý định câu hỏi và trả lời phù hợp
        /// </summary>
        private async Task<ChatResponse> DetectIntentAndRespond(string message)
        {
            // 1. CHÀO HỎI
            if (ContainsAny(message, "xin chào", "hello", "hi", "chào", "hey"))
            {
                return new ChatResponse
                {
                    Message = "Xin chào! 👋 Tôi là trợ lý ảo của nhà hàng. Tôi có thể giúp gì cho bạn?\n\n" +
                             "Bạn có thể hỏi tôi về:\n" +
                             "• Giờ mở cửa\n" +
                             "• Menu món ăn\n" +
                             "• Địa chỉ & liên hệ\n" +
                             "• Đặt bàn",
                    Suggestions = new List<string>
                    {
                        "Quán mở cửa lúc mấy giờ?",
                        "Menu có những món gì?",
                        "Địa chỉ nhà hàng ở đâu?",
                        "Làm sao để đặt bàn?"
                    }
                };
            }

            // 2. GIỜ MỞ CỬA
            if (ContainsAny(message, "giờ", "mở cửa", "đóng cửa", "mấy giờ", "hoạt động", "làm việc"))
            {
                return new ChatResponse
                {
                    Message = "⏰ **Giờ hoạt động:**\n\n" +
                             "📅 **Thứ 2 - Thứ 6:** 10:00 - 22:00\n" +
                             "📅 **Thứ 7 - Chủ nhật:** 09:00 - 23:00\n" +
                             "🎉 **Lễ Tết:** 09:00 - 00:00\n\n" +
                             "💡 Quán luôn sẵn sàng phục vụ bạn!",
                    Suggestions = new List<string>
                    {
                        "Menu có những món gì?",
                        "Giá cả như thế nào?",
                        "Tôi muốn đặt bàn"
                    }
                };
            }

            // 3. MENU / MÓN ĂN
            if (ContainsAny(message, "menu", "món", "món ăn", "thực đơn", "có gì", "đặc sản"))
            {
                // Lấy danh sách món ăn từ database
                var menuItems = await _context.MenuItems
                    .Where(m => m.IsAvailable)
                    .Include(m => m.Category)
                    .OrderBy(m => m.Category.Name)
                    .Take(10)
                    .ToListAsync();

                var menuText = "🍽️ **Menu đặc biệt:**\n\n";

                var groupedMenu = menuItems.GroupBy(m => m.Category?.Name ?? "Khác");
                foreach (var group in groupedMenu)
                {
                    menuText += $"**{group.Key}:**\n";
                    foreach (var item in group)
                    {
                        menuText += $"  • {item.Name} - {item.Price:N0}đ\n";
                    }
                    menuText += "\n";
                }

                menuText += "💡 Xem menu đầy đủ tại trang **Menu** của chúng tôi!";

                return new ChatResponse
                {
                    Message = menuText,
                    Suggestions = new List<string>
                    {
                        "Món nào ngon nhất?",
                        "Giá trung bình bao nhiêu?",
                        "Tôi muốn đặt bàn"
                    }
                };
            }

            // 4. ĐỊA CHỈ / LIÊN HỆ
            if (ContainsAny(message, "địa chỉ", "ở đâu", "liên hệ", "số điện thoại", "phone", "hotline", "chỉ đường"))
            {
                return new ChatResponse
                {
                    Message = "📍 **Thông tin liên hệ:**\n\n" +
                             "🏠 **Địa chỉ:** 123 Đường ABC, Quận 1, TP.HCM\n" +
                             "📞 **Hotline:** 0901 234 567\n" +
                             "📧 **Email:** contact@restaurant.com\n" +
                             "🌐 **Facebook:** fb.com/restaurant\n\n" +
                             "🗺️ Bạn có thể xem bản đồ tại trang **Liên hệ**!",
                    Suggestions = new List<string>
                    {
                        "Cách đặt bàn như thế nào?",
                        "Menu có những gì?",
                        "Quán có giao hàng không?"
                    }
                };
            }

            // 5. ĐẶT BÀN
            if (ContainsAny(message, "đặt bàn", "book", "reservation", "đặt chỗ", "order"))
            {
                return new ChatResponse
                {
                    Message = "📅 **Đặt bàn rất dễ dàng:**\n\n" +
                             "**Cách 1: Online**\n" +
                             "1. Vào trang **Đặt bàn**\n" +
                             "2. Chọn ngày, giờ, số người\n" +
                             "3. Điền thông tin liên hệ\n" +
                             "4. Xác nhận đặt bàn\n\n" +
                             "**Cách 2: Gọi điện**\n" +
                             "📞 Hotline: **0901 234 567**\n\n" +
                             "💡 Đặt trước 2 giờ để được phục vụ tốt nhất!",
                    Suggestions = new List<string>
                    {
                        "Đặt bàn có mất phí không?",
                        "Đặt bàn cho 10 người được không?",
                        "Có thể hủy đặt bàn không?"
                    }
                };
            }

            // 6. GIÁ CẢ
            if (ContainsAny(message, "giá", "bao nhiêu", "tiền", "chi phí", "price"))
            {
                var avgPrice = await _context.MenuItems
                    .Where(m => m.IsAvailable)
                    .AverageAsync(m => m.Price);

                return new ChatResponse
                {
                    Message = $"💰 **Giá cả:**\n\n" +
                             $"📊 **Giá trung bình:** {avgPrice:N0}đ/món\n" +
                             $"🍜 **Món phổ biến:** 45.000đ - 120.000đ\n" +
                             $"🥘 **Món đặc biệt:** 150.000đ - 300.000đ\n\n" +
                             $"💡 Giá đã bao gồm VAT. Không tính phí phục vụ!",
                    Suggestions = new List<string>
                    {
                        "Có chương trình khuyến mãi không?",
                        "Menu có những món gì?",
                        "Tôi muốn đặt bàn"
                    }
                };
            }

            // 7. KHUYẾN MÃI / GIẢM GIÁ
            if (ContainsAny(message, "khuyến mãi", "giảm giá", "voucher", "sale", "ưu đãi", "promotion"))
            {
                return new ChatResponse
                {
                    Message = "🎉 **Chương trình khuyến mãi:**\n\n" +
                             "🎁 **Sinh nhật:** Giảm 20% cho khách có sinh nhật\n" +
                             "👥 **Nhóm đông:** Giảm 10% cho bàn từ 10 người\n" +
                             "💳 **Thẻ thành viên:** Tích điểm, đổi quà\n" +
                             "📅 **Happy Hour:** Giảm 15% từ 14h-16h hàng ngày\n\n" +
                             "💡 Theo dõi Facebook để cập nhật ưu đãi mới!",
                    Suggestions = new List<string>
                    {
                        "Làm thẻ thành viên như thế nào?",
                        "Tôi muốn đặt bàn",
                        "Menu có những món gì?"
                    }
                };
            }

            // 8. GIAO HÀNG
            if (ContainsAny(message, "giao hàng", "ship", "delivery", "đặt online", "mang về"))
            {
                return new ChatResponse
                {
                    Message = "🚗 **Dịch vụ giao hàng:**\n\n" +
                             "✅ **Có giao hàng** trong bán kính 5km\n" +
                             "🆓 **Miễn phí ship** đơn từ 200.000đ\n" +
                             "⏱️ **Thời gian:** 30-45 phút\n" +
                             "📦 **Đóng gói:** Cẩn thận, đảm bảo chất lượng\n\n" +
                             "📞 Gọi **0901 234 567** để đặt giao hàng!",
                    Suggestions = new List<string>
                    {
                        "Menu có những món gì?",
                        "Giá giao hàng là bao nhiêu?",
                        "Quán mở cửa lúc mấy giờ?"
                    }
                };
            }

            // 9. TRẢ LỜI MẶC ĐỊNH (không hiểu câu hỏi)
            return new ChatResponse
            {
                Message = "🤔 Xin lỗi, tôi chưa hiểu câu hỏi của bạn.\n\n" +
                         "Bạn có thể hỏi tôi về:\n" +
                         "• ⏰ Giờ mở cửa\n" +
                         "• 🍽️ Menu món ăn\n" +
                         "• 📍 Địa chỉ & liên hệ\n" +
                         "• 📅 Đặt bàn\n" +
                         "• 💰 Giá cả\n" +
                         "• 🎉 Khuyến mãi\n" +
                         "• 🚗 Giao hàng\n\n" +
                         "Hoặc gọi **0901 234 567** để được hỗ trợ trực tiếp!",
                Suggestions = new List<string>
                {
                    "Quán mở cửa lúc mấy giờ?",
                    "Menu có những món gì?",
                    "Địa chỉ nhà hàng ở đâu?",
                    "Tôi muốn đặt bàn"
                }
            };
        }

        /// <summary>
        /// Helper method: Kiểm tra message có chứa bất kỳ từ khóa nào không
        /// </summary>
        private bool ContainsAny(string message, params string[] keywords)
        {
            return keywords.Any(keyword => message.Contains(keyword));
        }
    }

    #region DTO Classes

    /// <summary>
    /// Request từ frontend (tin nhắn của user)
    /// </summary>
    public class ChatMessage
    {
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response trả về frontend (câu trả lời + gợi ý)
    /// </summary>
    public class ChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string> Suggestions { get; set; } = new();
        public object? Data { get; set; }
    }

    #endregion
}
