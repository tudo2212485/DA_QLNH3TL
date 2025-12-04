
using QLNHWebApp.Models;
using QLNHWebApp.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

try
{
    Console.WriteLine("=== APPLICATION STARTING ===");

    // ===== SERILOG CONFIGURATION ===== (Temporarily disabled)
    /*
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build())
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
    */

    Console.WriteLine("Creating WebApplication builder...");
    var builder = WebApplication.CreateBuilder(args);
    // builder.Host.UseSerilog(); // Use Serilog instead of default logging (Disabled)

    Console.WriteLine("Adding services...");
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "API Quản Lý Nhà Hàng",
            Version = "v1.0",
            Description = "RESTful API cho hệ thống quản lý nhà hàng - ASP.NET Core 9",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Your Name",
                Email = "your@email.com"
            }
        });
    });
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromHours(2);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Register EmailService
    // builder.Services.AddScoped<IEmailService, EmailService>(); // Temporary comment

    // Add Authentication
    builder.Services.AddAuthentication("AdminAuth")
        .AddCookie("AdminAuth", options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

    builder.Services.AddAuthorization(options =>
    {
        // Policy 1: Chỉ Admin (Quản lý nhân viên, Thống kê doanh thu, Cấu hình hệ thống)
        options.AddPolicy("AdminOnly", policy =>
            policy.RequireRole("Admin"));

        // Policy 2: Admin + Staff (Đặt bàn, Gọi món, Xử lý order, Quản lý bàn)
        // ĐÃ XÓA: "Đầu bếp" - Hệ thống chỉ còn 2 role: Admin và Staff
        options.AddPolicy("AdminAndStaff", policy =>
            policy.RequireRole("Admin", "Staff", "Nhân viên")); // Support both "Staff" and legacy "Nhân viên"

        // Policy 3: Tất cả roles (bao gồm Customer)
        options.AddPolicy("AllRoles", policy =>
            policy.RequireRole("Admin", "Staff", "Nhân viên", "Customer"));
    });

    // Add DataSeeder service
    builder.Services.AddScoped<QLNHWebApp.Services.DataSeederService>(); // Re-enabled for AuthController
    builder.Services.AddScoped<TableAvailabilityService>(); // Table availability checking service
    builder.Services.AddDbContext<RestaurantDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    Console.WriteLine("Building application...");
    var app = builder.Build();

    Console.WriteLine("Configuring middleware...");
    // ===== GLOBAL EXCEPTION HANDLER =====
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            // Log exception (có thể thêm logger sau)
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {exception?.Message}");
            Console.WriteLine($"[STACK] {exception?.StackTrace}");

            var response = new
            {
                success = false,
                message = app.Environment.IsDevelopment()
                    ? $"Lỗi: {exception?.Message}"
                    : "Đã xảy ra lỗi. Vui lòng thử lại sau.",
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await context.Response.WriteAsJsonAsync(response);
        });
    });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.UseHttpsRedirection(); // Tạm thời tắt để chạy HTTP only
    app.UseCors("AllowReactApp");
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseSession();

    // Map MVC routes first (for admin and auth controllers)
    app.MapControllerRoute(
        name: "admin",
        pattern: "Admin/{action=Dashboard}/{id?}",
        defaults: new { controller = "Admin" });

    app.MapControllerRoute(
        name: "auth",
        pattern: "Auth/{action=Login}/{id?}",
        defaults: new { controller = "Auth" });

    // Map specific controllers with constraints BEFORE default route
    app.MapControllerRoute(
        name: "mvc",
        pattern: "{controller}/{action=Index}/{id?}",
        constraints: new { controller = "^(Payment|Booking|AdminBooking|AdminCustomer|AdminMenu|OrderManagement|Settings|Table|Test)$" });

    // Minimal API endpoint để test thêm món ăn vào booking
    app.MapGet("/api/test/add-items/{bookingId:int}", async (int bookingId, RestaurantDbContext context) =>
    {
        try
        {
            var booking = await context.TableBookings.FindAsync(bookingId);
            if (booking == null)
                return Results.Text($"❌ Booking #{bookingId} không tồn tại!");

            var existingCount = await context.OrderItems.CountAsync(oi => oi.TableBookingId == bookingId);
            if (existingCount > 0)
                return Results.Text($"ℹ️ Booking #{bookingId} đã có {existingCount} món ăn rồi!");

            var items = new List<OrderItem>
            {
            new OrderItem { MenuItemId = 2, Quantity = 2, Price = 95000, TableBookingId = bookingId },
            new OrderItem { MenuItemId = 1, Quantity = 3, Price = 35000, TableBookingId = bookingId },
            new OrderItem { MenuItemId = 3, Quantity = 1, Price = 120000, TableBookingId = bookingId },
            new OrderItem { MenuItemId = 6, Quantity = 4, Price = 40000, TableBookingId = bookingId }
            };

            context.OrderItems.AddRange(items);
            await context.SaveChangesAsync();

            var result = $"✅ ĐÃ THÊM {items.Count} MÓN ĂN VÀO BOOKING #{bookingId}\n\n";
            result += $"Khách hàng: {booking.CustomerName}\n\n";
            result += "DANH SÁCH MÓN:\n";
            result += "- Bò lúc lắc: 2 x 95,000đ = 190,000đ\n";
            result += "- Gỏi cuốn: 3 x 35,000đ = 105,000đ\n";
            result += "- Sườn nướng: 1 x 120,000đ = 120,000đ\n";
            result += "- Trà đào: 4 x 40,000đ = 160,000đ\n";
            result += "\nTỔNG: 575,000đ\n\n";
            result += $"👉 Refresh trang: http://localhost:5000/AdminBooking/Details/{bookingId}";

            return Results.Text(result);
        }
        catch (Exception ex)
        {
            return Results.Text($"❌ Lỗi: {ex.Message}");
        }
    });

    // Map API controllers (for API endpoints)  
    app.MapControllers();

    // Serve static files for React app AFTER all routes
    app.UseDefaultFiles();
    app.UseStaticFiles();

    // Default route for Home (React app) - LAST priority, only for Home controller
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}",
        constraints: new { controller = "^Home$" });

    // Fallback to React app for client-side routing (only for unmatched routes)
    // app.MapFallbackToFile("index.html"); // Tạm thời comment để debug

    // Seed initial data - Temporarily disabled to allow app to start
    /*
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<QLNHWebApp.Services.DataSeederService>();
        await seeder.SeedAsync();
    }
    */

    Console.WriteLine("Starting application...");
    Console.WriteLine("Listening on: http://localhost:5000");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
    Console.WriteLine($"STACK TRACE: {ex.StackTrace}");
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
