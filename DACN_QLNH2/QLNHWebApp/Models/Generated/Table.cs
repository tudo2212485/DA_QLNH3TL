using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class Table
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Floor { get; set; } = null!;

    public int Capacity { get; set; }

    public string Description { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public int IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<TableBooking> TableBookings { get; set; } = new List<TableBooking>();
}
