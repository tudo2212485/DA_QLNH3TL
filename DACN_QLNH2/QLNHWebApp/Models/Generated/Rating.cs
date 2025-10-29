using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class Rating
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public int Score { get; set; }

    public string Comment { get; set; } = null!;

    public string Date { get; set; } = null!;

    public virtual MenuItem MenuItem { get; set; } = null!;
}
