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
        /// Test endpoint để kiểm tra database
        /// Endpoint: GET /api/ChatBotApi/test
        /// </summary>
        [HttpGet("test")]
        public async Task<IActionResult> TestDatabase()
        {
            var menuItems = await _context.MenuItems.ToListAsync();
            return Ok(new
            {
                count = menuItems.Count,
                items = menuItems.Select(m => new { m.Name, m.Price, m.Category })
            });
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
            if (ContainsAny(message, "menu", "món", "món ăn", "thực đơn", "có gì", "đặc sản", "có món nào", "những món", "món nào", "ăn gì", "bán gì"))
            {
                // Lấy danh sách món ăn từ database (load first, then sort in memory for SQLite)
                var menuItems = (await _context.MenuItems.ToListAsync())
                    .OrderBy(m => m.Category)
                    .ThenBy(m => m.Price)
                    .ToList();

                if (!menuItems.Any())
                {
                    return new ChatResponse
                    {
                        Message = "⚠️ Hiện tại chưa có món ăn nào trong hệ thống.\n\n" +
                                 "Vui lòng liên hệ: **0901 234 567** để biết thêm chi tiết!",
                        Suggestions = new List<string>
                        {
                            "Quán mở cửa lúc mấy giờ?",
                            "Địa chỉ nhà hàng ở đâu?",
                            "Cách đặt bàn?"
                        }
                    };
                }

                var menuText = $"🍽️ **Menu Nhà Hàng 3TL** ({menuItems.Count} món):\n\n";

                var groupedMenu = menuItems.GroupBy(m => m.Category ?? "Khác");
                foreach (var group in groupedMenu)
                {
                    menuText += $"📌 **{group.Key.ToUpper()}**\n";
                    foreach (var item in group)
                    {
                        menuText += $"  • **{item.Name}** - {item.Price:N0}đ\n";
                        if (!string.IsNullOrEmpty(item.Description))
                        {
                            menuText += $"    _{item.Description}_\n";
                        }
                    }
                    menuText += "\n";
                }

                menuText += "💡 **Xem ảnh & đặt món** tại trang **Thực đơn** của chúng tôi!";

                return new ChatResponse
                {
                    Message = menuText,
                    Suggestions = new List<string>
                    {
                        "Món nào ngon nhất?",
                        "Giá trung bình bao nhiêu?",
                        "Tôi muốn đặt bàn"
                    },
                    Data = new { totalItems = menuItems.Count, categories = groupedMenu.Select(g => g.Key).ToList() }
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

            // 5. HỦY ĐẶT BÀN
            if (ContainsAny(message, "hủy đặt bàn", "hủy bàn", "hủy reservation", "cancel booking", "huỷ đặt", "không đi nữa"))
            {
                return new ChatResponse
                {
                    Message = "🔄 **Hủy đặt bàn:**\n\n" +
                             "**Cách 1: Gọi điện**\n" +
                             "📞 Hotline: **0901 234 567**\n" +
                             "⏰ Gọi trong giờ hoạt động: 10:00 - 22:00\n\n" +
                             "**Cách 2: Gửi email**\n" +
                             "📧 Email: **contact@restaurant.com**\n" +
                             "✉️ Tiêu đề: \"Hủy đặt bàn - [Tên của bạn]\"\n" +
                             "📝 Nội dung: Thông tin đặt bàn cần hủy\n\n" +
                             "⚠️ **Lưu ý:** Vui lòng hủy trước **2 giờ** để tránh phí phạt!",
                    Suggestions = new List<string>
                    {
                        "Có bị phạt không?",
                        "Tôi muốn đặt bàn mới",
                        "Liên hệ hotline"
                    }
                };
            }

            // 6. ĐẶT BÀN
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
                             "💡 Đặt trước 2 giờ để được phục vụ tốt nhất!\n" +
                             "ℹ️ Cần hủy? Gọi hotline hoặc gửi email!",
                    Suggestions = new List<string>
                    {
                        "Có thể hủy đặt bàn không?",
                        "Đặt bàn cho 10 người được không?",
                        "Menu có những gì?"
                    }
                };
            }

            // 6. GIÁ CẢ
            if (ContainsAny(message, "giá", "bao nhiêu", "tiền", "chi phí", "price", "giá cả", "giá tiền", "tốn bao nhiêu", "hết bao nhiêu"))
            {
                var menuItems = await _context.MenuItems.ToListAsync();

                if (!menuItems.Any())
                {
                    return new ChatResponse
                    {
                        Message = "⚠️ Hiện chưa có thông tin giá món ăn.\n\n" +
                                 "📞 Vui lòng liên hệ: **0901 234 567**",
                        Suggestions = new List<string> { "Menu có gì?", "Quán mở cửa lúc mấy giờ?" }
                    };
                }

                var avgPrice = menuItems.Average(m => m.Price);
                var minPrice = menuItems.Min(m => m.Price);
                var maxPrice = menuItems.Max(m => m.Price);

                // Top 5 món rẻ nhất
                var cheapestItems = menuItems.OrderBy(m => m.Price).Take(5);
                var cheapestText = string.Join("\n", cheapestItems.Select(m => $"  • {m.Name} - {m.Price:N0}đ"));

                // Top 5 món đắt nhất
                var expensiveItems = menuItems.OrderByDescending(m => m.Price).Take(5);
                var expensiveText = string.Join("\n", expensiveItems.Select(m => $"  • {m.Name} - {m.Price:N0}đ"));

                return new ChatResponse
                {
                    Message = $"💰 **Bảng Giá Nhà Hàng:**\n\n" +
                             $"📊 **Giá trung bình:** {avgPrice:N0}đ/món\n" +
                             $"💵 **Giá thấp nhất:** {minPrice:N0}đ\n" +
                             $"💎 **Giá cao nhất:** {maxPrice:N0}đ\n\n" +
                             $"🏷️ **Top 5 món giá rẻ:**\n{cheapestText}\n\n" +
                             $"⭐ **Top 5 món đặc biệt:**\n{expensiveText}\n\n" +
                             $"💡 Giá đã bao gồm VAT. Không phí phục vụ!",
                    Suggestions = new List<string>
                    {
                        "Có khuyến mãi không?",
                        "Xem menu đầy đủ",
                        "Tôi muốn đặt bàn"
                    },
                    Data = new { avgPrice, minPrice, maxPrice, totalItems = menuItems.Count }
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

            // 9. MÓN NGON / RECOMMEND
            if (ContainsAny(message, "món ngon", "ngon nhất", "đặc biệt", "nên ăn", "gợi ý", "recommend", "best", "đặc sản"))
            {
                // Load data first, then sort in memory (SQLite doesn't support decimal ORDER BY)
                var topItems = (await _context.MenuItems.ToListAsync())
                    .OrderByDescending(m => m.Price)
                    .Take(5)
                    .ToList();

                if (!topItems.Any())
                {
                    return new ChatResponse
                    {
                        Message = "⚠️ Hiện chưa có thông tin món ăn.\n\n📞 Liên hệ: **0901 234 567**",
                        Suggestions = new List<string> { "Quán mở cửa lúc mấy giờ?", "Địa chỉ ở đâu?" }
                    };
                }

                var recommendText = "⭐ **Món đặc biệt chúng tôi gợi ý:**\n\n";
                foreach (var item in topItems)
                {
                    recommendText += $"🍴 **{item.Name}** - {item.Price:N0}đ\n";
                    if (!string.IsNullOrEmpty(item.Description))
                    {
                        recommendText += $"   _{item.Description}_\n";
                    }
                    recommendText += "\n";
                }
                recommendText += "💡 Tất cả đều là món signature của chúng tôi!";

                return new ChatResponse
                {
                    Message = recommendText,
                    Suggestions = new List<string>
                    {
                        "Xem menu đầy đủ",
                        "Giá trung bình bao nhiêu?",
                        "Tôi muốn đặt bàn"
                    }
                };
            }

            // 10. SỐ LƯỢNG MÓN / THỐNG KÊ
            if (ContainsAny(message, "bao nhiêu món", "có mấy món", "tổng cộng", "thống kê", "số lượng"))
            {
                var totalItems = await _context.MenuItems.CountAsync();
                var categories = await _context.MenuItems
                    .Select(m => m.Category)
                    .Distinct()
                    .ToListAsync();

                var categoryText = string.Join(", ", categories.Select(c => $"**{c}**"));

                return new ChatResponse
                {
                    Message = $"📊 **Thống kê menu:**\n\n" +
                             $"🍽️ **Tổng số món:** {totalItems} món\n" +
                             $"📂 **Danh mục:** {categoryText}\n" +
                             $"✅ **Cập nhật:** Hàng ngày\n\n" +
                             $"💡 Xem chi tiết tại trang **Thực đơn**!",
                    Suggestions = new List<string>
                    {
                        "Xem menu đầy đủ",
                        "Món nào ngon nhất?",
                        "Giá cả như thế nào?"
                    }
                };
            }

            // 11. CẢM ƠN
            if (ContainsAny(message, "cảm ơn", "thanks", "thank you", "cám ơn", "ok", "được rồi", "oke"))
            {
                return new ChatResponse
                {
                    Message = "😊 **Rất vui được hỗ trợ bạn!**\n\n" +
                             "Nếu còn câu hỏi nào khác, đừng ngại hỏi nhé!\n\n" +
                             "📞 Hotline: **0901 234 567**\n" +
                             "🌐 Website: **nha-hang-3tl.com**",
                    Suggestions = new List<string>
                    {
                        "Tôi muốn đặt bàn",
                        "Xem menu",
                        "Giờ mở cửa"
                    }
                };
            }

            // 12. TẠM BIỆT
            if (ContainsAny(message, "tạm biệt", "bye", "goodbye", "hẹn gặp lại", "thôi", "quit"))
            {
                return new ChatResponse
                {
                    Message = "👋 **Tạm biệt! Hẹn gặp lại bạn!**\n\n" +
                             "Cảm ơn bạn đã quan tâm đến Nhà Hàng 3TL.\n" +
                             "Chúc bạn một ngày tuyệt vời! 🌟\n\n" +
                             "📞 **0901 234 567** - Luôn sẵn sàng phục vụ!",
                    Suggestions = new List<string>
                    {
                        "Xin chào",
                        "Menu có gì?",
                        "Đặt bàn"
                    }
                };
            }

            // 13. TRẢ LỜI MẶC ĐỊNH (không hiểu câu hỏi)
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
