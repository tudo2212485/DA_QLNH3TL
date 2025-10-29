using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models.Generated;

namespace QLNHWebApp.Data;

public partial class QlnhdbContext : DbContext
{
    public QlnhdbContext()
    {
    }

    public QlnhdbContext(DbContextOptions<QlnhdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboItem> ComboItems { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<RestaurantSetting> RestaurantSettings { get; set; }

    public virtual DbSet<Table> Tables { get; set; }

    public virtual DbSet<TableBooking> TableBookings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=../data/QLNHDB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ComboItem>(entity =>
        {
            entity.HasIndex(e => e.ComboId, "IX_ComboItems_ComboId");

            entity.HasIndex(e => e.MenuItemId, "IX_ComboItems_MenuItemId");

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboItems).HasForeignKey(d => d.ComboId);

            entity.HasOne(d => d.MenuItem).WithMany(p => p.ComboItems).HasForeignKey(d => d.MenuItemId);
        });

        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Email).HasDefaultValue("");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasIndex(e => e.TableId, "IX_Orders_TableId");

            entity.HasOne(d => d.Table).WithMany(p => p.Orders).HasForeignKey(d => d.TableId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasIndex(e => e.MenuItemId, "IX_OrderItems_MenuItemId");

            entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderId");

            entity.HasIndex(e => e.TableBookingId, "IX_OrderItems_TableBookingId");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.OrderItems).HasForeignKey(d => d.MenuItemId);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.TableBooking).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.TableBookingId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasIndex(e => e.MenuItemId, "IX_Ratings_MenuItemId");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.Ratings).HasForeignKey(d => d.MenuItemId);
        });

        modelBuilder.Entity<TableBooking>(entity =>
        {
            entity.HasIndex(e => e.TableId, "IX_TableBookings_TableId");

            entity.HasOne(d => d.Table).WithMany(p => p.TableBookings).HasForeignKey(d => d.TableId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
