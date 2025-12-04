namespace QLNHWebApp.Models
{
    /// <summary>
    /// ViewModel chứa toàn bộ dữ liệu thống kê cho Dashboard
    /// </summary>
    public class DashboardViewModel
    {
        // === KPI CARDS (Thẻ thống kê) ===

        /// <summary>
        /// ĐƠN HÀNG THÁNG NÀY (Orders This Month) - Số lượng đơn được tạo trong tháng hiện tại
        /// </summary>
        public int OrdersThisMonth { get; set; }

        /// <summary>
        /// Số đơn hàng tháng trước (để tính % tăng trưởng)
        /// </summary>
        public int OrdersLastMonth { get; set; }

        /// <summary>
        /// Tỷ lệ tăng trưởng số đơn hàng (%) so với tháng trước
        /// </summary>
        public decimal OrdersGrowthRate { get; set; }

        /// <summary>
        /// Tổng doanh thu từ các đơn đã thanh toán (ALL TIME)
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// Doanh thu của tháng hiện tại (chỉ tính đơn "Đã thanh toán")
        /// </summary>
        public decimal CurrentMonthRevenue { get; set; }

        /// <summary>
        /// Doanh thu của tháng trước
        /// </summary>
        public decimal LastMonthRevenue { get; set; }

        /// <summary>
        /// Tỷ lệ tăng trưởng doanh thu (%) so với tháng trước
        /// VD: +15.5 (tăng 15.5%), -8.2 (giảm 8.2%)
        /// </summary>
        public decimal MonthlyGrowthRate { get; set; }

        /// <summary>
        /// Doanh thu hôm nay
        /// </summary>
        public decimal TodayRevenue { get; set; }

        /// <summary>
        /// Số đơn hàng hôm nay
        /// </summary>
        public int TodayOrders { get; set; }

        // === REVENUE CHART DATA (Biểu đồ doanh thu) ===

        /// <summary>
        /// Danh sách nhãn tháng (Labels) cho biểu đồ - 12 tháng gần nhất
        /// Format: "Tháng 1/2025", "Tháng 2/2025", ...
        /// </summary>
        public List<string> RevenueLabels { get; set; } = new List<string>();

        /// <summary>
        /// Dữ liệu doanh thu tương ứng với từng tháng (12 phần tử)
        /// Quan trọng: Tháng nào không có doanh thu = 0 (không để null)
        /// </summary>
        public List<decimal> RevenueData { get; set; } = new List<decimal>();

        /// <summary>
        /// Số lượng đơn hàng theo từng tháng (12 phần tử)
        /// </summary>
        public List<int> OrderCountData { get; set; } = new List<int>();

        // === ORDER STATUS DATA (Biểu đồ trạng thái đơn hàng) ===

        /// <summary>
        /// Danh sách trạng thái đơn hàng
        /// VD: ["Đã thanh toán", "Đang phục vụ", "Đã hủy"]
        /// </summary>
        public List<string> StatusLabels { get; set; } = new List<string>();

        /// <summary>
        /// Số lượng đơn theo từng trạng thái
        /// </summary>
        public List<int> StatusData { get; set; } = new List<int>();

        // === TOP MENU ITEMS DATA (Biểu đồ món ăn bán chạy) ===

        /// <summary>
        /// Tên các món ăn bán chạy nhất (Top 5)
        /// </summary>
        public List<string> TopMenuLabels { get; set; } = new List<string>();

        /// <summary>
        /// Số lượng đã bán của từng món
        /// </summary>
        public List<int> TopMenuQuantities { get; set; } = new List<int>();

        /// <summary>
        /// Doanh thu từ từng món ăn
        /// </summary>
        public List<decimal> TopMenuRevenue { get; set; } = new List<decimal>();

        // === USER INFO ===

        /// <summary>
        /// Tên đầy đủ của người dùng đang đăng nhập
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Role của người dùng (Admin, Staff)
        /// </summary>
        public string? UserRole { get; set; }
    }
}
