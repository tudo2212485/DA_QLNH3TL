using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class ContactMessage
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime Date { get; set; }
}
