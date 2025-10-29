using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class TableBooking
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime BookingDate { get; set; }

    public string BookingTime { get; set; } = null!;

    public int Guests { get; set; }

    public string? Note { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int TableId { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Table Table { get; set; } = null!;
}
