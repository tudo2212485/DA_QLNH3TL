using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class Order
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = null!;

    public DateTime Date { get; set; }

    public int Guests { get; set; }

    public string? Note { get; set; }

    public string Phone { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int? TableId { get; set; }

    public string Time { get; set; } = null!;

    public decimal TotalPrice { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Table? Table { get; set; }
}
