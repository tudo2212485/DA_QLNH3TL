using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class ComboItem
{
    public int Id { get; set; }

    public int ComboId { get; set; }

    public int MenuItemId { get; set; }

    public int Quantity { get; set; }

    public virtual Combo Combo { get; set; } = null!;

    public virtual MenuItem MenuItem { get; set; } = null!;
}
