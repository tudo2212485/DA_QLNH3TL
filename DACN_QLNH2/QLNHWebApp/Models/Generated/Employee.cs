using System;
using System.Collections.Generic;

namespace QLNHWebApp.Models.Generated;

public partial class Employee
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int IsActive { get; set; }

    public string Email { get; set; } = null!;
}
