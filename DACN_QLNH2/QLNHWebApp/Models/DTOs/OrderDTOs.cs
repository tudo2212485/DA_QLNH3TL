namespace QLNHWebApp.Models.DTOs;

/// <summary>
/// DTO cho request thêm món vào order
/// </summary>
public class AddItemRequest
{
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO cho request xóa món khỏi order
/// </summary>
public class RemoveItemRequest
{
    public int OrderId { get; set; }
    public int OrderItemId { get; set; }
}

/// <summary>
/// DTO cho request cập nhật số lượng món
/// </summary>
public class UpdateQuantityRequest
{
    public int OrderItemId { get; set; }
    public int Quantity { get; set; }
}

/// <summary>
/// DTO cho request thay đổi bàn
/// </summary>
public class ChangeTableRequest
{
    public int OrderId { get; set; }
    public int NewTableId { get; set; }
}

/// <summary>
/// DTO cho response thao tác với order
/// </summary>
public class OrderActionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal? NewTotalPrice { get; set; }
}









