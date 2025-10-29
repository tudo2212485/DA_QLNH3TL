using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace QLNHWebApp.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
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
        public string CustomerName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Ngày đặt bàn là bắt buộc")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Giờ đặt bàn là bắt buộc")]
        public string Time { get; set; } = string.Empty;
        [Required(ErrorMessage = "Số khách là bắt buộc")]
        [Range(1, 50, ErrorMessage = "Số khách phải từ 1 đến 50")]
        public int Guests { get; set; }
        public string? Note { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Chờ xác nhận";
        public int? TableId { get; set; }
        public Table? Table { get; set; }
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }  // Nullable để có thể thuộc về TableBooking trước khi có Order
        public Order? Order { get; set; }
        public int? TableBookingId { get; set; }  // Thêm để liên kết với TableBooking
        public TableBooking? TableBooking { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class ContactMessage
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
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
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = string.Empty; // Admin, Waiter, Chef, Cashier
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
