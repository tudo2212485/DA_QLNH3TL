namespace QLNHWebApp.Models.DTOs;

/// <summary>
/// DTO cho request đặt bàn từ client
/// </summary>
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

/// <summary>
/// DTO cho món ăn trong đơn đặt bàn
/// </summary>
public class BookingOrderItem
{
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO cho đặt bàn trực tiếp (walk-in)
/// </summary>
public class WalkInBookingRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Guests { get; set; }
    public int TableId { get; set; }
    public string? Note { get; set; }
}

/// <summary>
/// DTO cho response khi đặt bàn thành công
/// </summary>
public class BookingResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? BookingId { get; set; }
    public int? OrderId { get; set; }
}









