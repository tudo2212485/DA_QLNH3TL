using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Services
{
    public class DataSeederService
    {
        private readonly RestaurantDbContext _context;

        public DataSeederService(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Seed Employees nếu chưa có
            if (!_context.Employees.Any())
            {
                var employees = new[]
                {
                    new Employee 
                    { 
                        FullName = "Admin Hệ Thống", 
                        Username = "admin", 
                        Email = "admin@restaurant.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                        Role = "Admin",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    },
                    new Employee 
                    { 
                        FullName = "Nguyễn Văn A", 
                        Username = "waiter1", 
                        Email = "waiter1@restaurant.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), 
                        Role = "Waiter",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    },
                    new Employee 
                    { 
                        FullName = "Trần Thị B", 
                        Username = "chef1", 
                        Email = "chef1@restaurant.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), 
                        Role = "Chef",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    }
                };

                _context.Employees.AddRange(employees);
            }

            // Seed RestaurantSettings nếu chưa có
            if (!_context.RestaurantSettings.Any())
            {
                var settings = new RestaurantSettings
                {
                    RestaurantName = "Nhà Hàng 3TL",
                    Address = "123 Đường ABC, Quận 1, TP.HCM",
                    Phone = "028.1234.5678",
                    Email = "info@3tlrestaurant.com",
                    OpenTime = new TimeOnly(10, 0),
                    CloseTime = new TimeOnly(22, 0),
                    TaxRate = 0.1M
                };

                _context.RestaurantSettings.Add(settings);
            }

            // Seed Tables nếu chưa có
            if (!_context.Tables.Any())
            {
                var tables = new List<Table>();

                // Tầng 1: 8 bàn cho 1-20 khách
                for (int i = 1; i <= 8; i++)
                {
                    tables.Add(new Table
                    {
                        Name = $"Bàn {i}",
                        Floor = "Tầng 1",
                        Capacity = i <= 4 ? 4 : (i <= 6 ? 8 : 20), // Bàn 1-4: 4 người, 5-6: 8 người, 7-8: 20 người
                        Description = i <= 4 ? "Bàn 4 người" : (i <= 6 ? "Bàn 8 người" : "Bàn lớn 20 người"),
                        ImageUrl = "/images/placeholder-table.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                // Tầng 2: 7 bàn cho 1-15 khách
                for (int i = 1; i <= 7; i++)
                {
                    tables.Add(new Table
                    {
                        Name = $"Bàn {i + 8}", // Bàn 9-15
                        Floor = "Tầng 2",
                        Capacity = i <= 3 ? 4 : (i <= 5 ? 8 : 15), // Bàn 9-11: 4 người, 12-13: 8 người, 14-15: 15 người
                        Description = i <= 3 ? "Bàn 4 người" : (i <= 5 ? "Bàn 8 người" : "Bàn lớn 15 người"),
                        ImageUrl = "/images/placeholder-table.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                // Sân thượng: 4 bàn cho 1-10 khách
                for (int i = 1; i <= 4; i++)
                {
                    tables.Add(new Table
                    {
                        Name = $"Bàn {i + 15}", // Bàn 16-19
                        Floor = "Sân thượng",
                        Capacity = i <= 2 ? 4 : 10, // Bàn 16-17: 4 người, 18-19: 10 người
                        Description = i <= 2 ? "Bàn 4 người" : "Bàn lớn 10 người",
                        ImageUrl = "/images/placeholder-table.jpg",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    });
                }

                _context.Tables.AddRange(tables);
            }

            // Seed MenuItems nếu chưa có
            if (!_context.MenuItems.Any())
            {
                var menuItems = new[]
                {
                    new MenuItem 
                    { 
                        Name = "Gỏi cuốn", 
                        Description = "Gỏi cuốn tôm thịt tươi ngon", 
                        Price = 35000, 
                        Category = "Khai vị", 
                        ImageUrl = "/images/goicuon.jpg"
                    },
                    new MenuItem 
                    { 
                        Name = "Bò lúc lắc", 
                        Description = "Bò lúc lắc hạt điều thơm ngon", 
                        Price = 95000, 
                        Category = "Món chính", 
                        ImageUrl = "/images/boluclac.jpg"
                    },
                    new MenuItem 
                    { 
                        Name = "Sườn nướng", 
                        Description = "Sườn nướng BBQ đặc biệt", 
                        Price = 120000, 
                        Category = "Món chính", 
                        ImageUrl = "/images/suonnuong.jpg"
                    },
                    new MenuItem 
                    { 
                        Name = "Lẩu Thái", 
                        Description = "Lẩu Thái hải sản chua cay", 
                        Price = 250000, 
                        Category = "Món chính", 
                        ImageUrl = "/images/lautai.jpg"
                    },
                    new MenuItem 
                    { 
                        Name = "Chè khúc bạch", 
                        Description = "Chè khúc bạch mát lạnh", 
                        Price = 25000, 
                        Category = "Tráng miệng", 
                        ImageUrl = "/images/chekhucbach.jpg"
                    },
                    new MenuItem 
                    { 
                        Name = "Trà đào", 
                        Description = "Trà đào cam sả tươi mát", 
                        Price = 40000, 
                        Category = "Đồ uống", 
                        ImageUrl = "/images/tradao.jpg"
                    }
                };

                _context.MenuItems.AddRange(menuItems);
            }

            await _context.SaveChangesAsync();
        }
    }
}