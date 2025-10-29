using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class RestaurantSetting
{
    public int Id { get; set; }

    public string RestaurantName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public TimeSpan OpenTime { get; set; }

    public TimeSpan CloseTime { get; set; }

    public decimal TaxRate { get; set; }
}
