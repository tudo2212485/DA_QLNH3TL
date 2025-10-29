using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class OrderItem
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public int? OrderId { get; set; }

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int? TableBookingId { get; set; }

    public virtual MenuItem MenuItem { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual TableBooking? TableBooking { get; set; }
}
