using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLNHWebApp.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên món ăn là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên món ăn phải từ 2-100 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Mô tả không quá 500 ký tự")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Giá món ăn là bắt buộc")]
        [Range(1000, 10000000, ErrorMessage = "Giá phải từ 1,000đ đến 10,000,000đ")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [StringLength(50, ErrorMessage = "Danh mục không quá 50 ký tự")]
        public string Category { get; set; } = string.Empty;
        
        [Url(ErrorMessage = "URL hình ảnh không hợp lệ")]
        [StringLength(500, ErrorMessage = "URL không quá 500 ký tự")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [JsonIgnore]
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class Order
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên phải từ 2-100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Số điện thoại phải có 10-11 số")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ngày đặt bàn là bắt buộc")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Giờ đặt bàn là bắt buộc")]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Giờ không hợp lệ (HH:mm)")]
        public string Time { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Số khách là bắt buộc")]
        [Range(1, 50, ErrorMessage = "Số khách phải từ 1 đến 50")]
        public int Guests { get; set; }
        
        [StringLength(1000, ErrorMessage = "Ghi chú không quá 1000 ký tự")]
        public string? Note { get; set; }
        
        [Range(0, 1000000000, ErrorMessage = "Tổng tiền không hợp lệ")]
        public decimal TotalPrice { get; set; }
        
        [StringLength(50, ErrorMessage = "Trạng thái không quá 50 ký tự")]
        public string Status { get; set; } = "Chờ xác nhận";
        
        public int? TableId { get; set; }
        public Table? Table { get; set; }
        
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public int? TableBookingId { get; set; }
        public TableBooking? TableBooking { get; set; }
        
        [Required(ErrorMessage = "Món ăn là bắt buộc")]
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;
        
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; }
        
        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, 10000000, ErrorMessage = "Giá không hợp lệ")]
        public decimal Price { get; set; }
    }

    public class ContactMessage
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên phải từ 2-100 ký tự")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150, ErrorMessage = "Email không quá 150 ký tự")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Tin nhắn phải từ 10-2000 ký tự")]
        public string Message { get; set; } = string.Empty;
        
        public DateTime Date { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class Rating
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;
        [Range(1, 5)]
        public int Score { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }

    // Employee model với full name và role-based access
    public class Employee
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2-100 ký tự")]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ, số và _")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150, ErrorMessage = "Email không quá 150 ký tự")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [RegularExpression(@"^(Admin|Nhân viên|Đầu bếp)$", ErrorMessage = "Vai trò phải là Admin, Nhân viên hoặc Đầu bếp")]
        public string Role { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }

    // RestaurantSettings cho thiết lập hệ thống
    public class RestaurantSettings
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
        public decimal TaxRate { get; set; } = 0.1M; // 10% VAT
    }

    // Combo cho quản lý combo món ăn
    public class Combo
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<ComboItem> ComboItems { get; set; } = new List<ComboItem>();
    }

    // ComboItem - junction table cho Combo và MenuItem
    public class ComboItem
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public Combo Combo { get; set; } = null!;
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;
        public int Quantity { get; set; } = 1;
    }

    // Table model cho quản lý bàn
    public class Table
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty; // Tên bàn (VD: "Bàn 1", "Bàn VIP 1")
        [Required]
        public string Floor { get; set; } = string.Empty; // Tầng 1, Tầng 2, Sân thượng
        [Required]
        public int Capacity { get; set; } // Sức chứa tối đa
        public string Description { get; set; } = string.Empty; // Mô tả bàn
        public string ImageUrl { get; set; } = string.Empty; // Hình ảnh bàn
        public bool IsActive { get; set; } = true; // Trạng thái hoạt động
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [JsonIgnore]
        public ICollection<TableBooking> TableBookings { get; set; } = new List<TableBooking>();
    }

    // TableBooking model cho đặt bàn
    public class TableBooking
    {
        public int Id { get; set; }
        [Required]
        public string CustomerName { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public DateTime BookingDate { get; set; }
        [Required]
        public string BookingTime { get; set; } = string.Empty;
        [Required]
        public int Guests { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Foreign keys
        public int TableId { get; set; }
        public Table Table { get; set; } = null!;
        
        // Navigation properties
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
