using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class Combo
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Price { get; set; } = null!;

    public string ImageUrl { get; set; } = null!;

    public int IsActive { get; set; }

    public string CreatedAt { get; set; } = null!;

    public virtual ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
}
