namespace QLNHWebApp.Models.DTOs;

/// <summary>
/// DTO cho request xác nhận thanh toán
/// </summary>
public class ConfirmPaymentRequest
{
    public int OrderId { get; set; }
    public string PaymentMethod { get; set; } = "Tại quầy"; // Mặc định thanh toán tại quầy
}

/// <summary>
/// DTO cho response thanh toán
/// </summary>
public class PaymentResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? OrderId { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? PaymentUrl { get; set; } // Nếu thanh toán qua VNPAY
}









