using Microsoft.EntityFrameworkCore;

namespace QLNHWebApp.Models
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RestaurantSettings> RestaurantSettings { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<TableBooking> TableBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure relationships
            modelBuilder.Entity<ComboItem>()
                .HasKey(ci => ci.Id);
            
            modelBuilder.Entity<ComboItem>()
                .HasOne(ci => ci.Combo)
                .WithMany(c => c.ComboItems)
                .HasForeignKey(ci => ci.ComboId);
            
            modelBuilder.Entity<ComboItem>()
                .HasOne(ci => ci.MenuItem)
                .WithMany()
                .HasForeignKey(ci => ci.MenuItemId);
            
            // Configure Table relationships
            modelBuilder.Entity<TableBooking>()
                .HasOne(tb => tb.Table)
                .WithMany(t => t.TableBookings)
                .HasForeignKey(tb => tb.TableId);
            
            // Configure OrderItem relationships với TableBooking
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.TableBooking)
                .WithMany(tb => tb.OrderItems)
                .HasForeignKey(oi => oi.TableBookingId)
                .OnDelete(DeleteBehavior.SetNull);  // Khi xóa TableBooking, set TableBookingId = null
            
            // Configure OrderItem relationships với Order  
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Khi xóa Order, xóa OrderItems
                
            // Seed data mẫu cho MenuItems
            modelBuilder.Entity<MenuItem>().HasData(
                new MenuItem { Id = 1, Name = "Gỏi cuốn tôm thịt", Description = "Món khai vị truyền thống", Price = 35000, Category = "Món khai vị", ImageUrl = "/images/goicuon.jpg" },
                new MenuItem { Id = 2, Name = "Bò lúc lắc", Description = "Món chính hấp dẫn", Price = 95000, Category = "Món chính", ImageUrl = "/images/boluclac.jpg" },
                new MenuItem { Id = 3, Name = "Sườn nướng mật ong", Description = "Món nướng đặc biệt", Price = 120000, Category = "Món nướng", ImageUrl = "/images/suonnuong.jpg" },
                new MenuItem { Id = 4, Name = "Lẩu thái hải sản", Description = "Các món lẩu thơm ngon", Price = 250000, Category = "Các món lẩu", ImageUrl = "/images/lautai.jpg" },
                new MenuItem { Id = 5, Name = "Chè khúc bạch", Description = "Tráng miệng mát lạnh", Price = 30000, Category = "Tráng miệng", ImageUrl = "/images/chekhucbach.jpg" },
                new MenuItem { Id = 6, Name = "Trà đào cam sả", Description = "Đồ uống giải khát", Price = 40000, Category = "Đồ uống", ImageUrl = "/images/tradao.jpg" }
            );
            
            // Seed data cho Employees - sẽ seed thông qua DataSeeder class
            // modelBuilder.Entity<Employee>().HasData(...); 
            
            // Seed data cho RestaurantSettings - sẽ seed thông qua DataSeeder class
            // modelBuilder.Entity<RestaurantSettings>().HasData(...);  
        }
    }
}
