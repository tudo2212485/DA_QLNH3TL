
using QLNHWebApp.Models;
using QLNHWebApp.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ===== SERILOG CONFIGURATION =====
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(); // Use Serilog instead of default logging

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "🍽️ Restaurant Management API", 
        Version = "v1.0",
        Description = @"
# RESTful API - Hệ Thống Quản Lý Nhà Hàng

## Chức năng chính:
- **Menu Management**: CRUD món ăn, categories
- **Order Management**: Quản lý đơn hàng, order items
- **Table Booking**: Đặt bàn, kiểm tra availability
- **Contact**: Liên hệ, feedback

## Authentication:
- Cookie-based authentication với BCrypt password hashing
- Role-based access: Admin, Waiter, Chef, Cashier

## Database:
- SQLite với Entity Framework Core
- Seed data có sẵn
",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Restaurant Management Team",
            Email = "contact@restaurant.com"
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

// ===== ROLE-BASED AUTHORIZATION POLICIES (3 ROLES) =====
builder.Services.AddAuthorization(options =>
{
    // Policy 1: Chỉ Admin (quản lý nhân viên, thiết lập hệ thống)
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    
    // Policy 2: Admin + Nhân viên (tất cả trừ quản lý nhân viên & settings)
    options.AddPolicy("AdminAndStaff", policy => 
        policy.RequireRole("Admin", "Nhân viên"));
    
    // Policy 3: Tất cả (Admin, Nhân viên, Đầu bếp) - chỉ xem
    options.AddPolicy("AllRoles", policy => 
        policy.RequireRole("Admin", "Nhân viên", "Đầu bếp"));
});

// Add DataSeeder service
builder.Services.AddScoped<QLNHWebApp.Services.DataSeederService>();
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

var app = builder.Build();

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
// Enable Swagger for ALL environments (để dễ demo cho thầy)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Management API v1.0");
    c.RoutePrefix = "swagger"; // Swagger UI tại /swagger
    c.DocumentTitle = "🍽️ Restaurant API Documentation";
    c.DefaultModelsExpandDepth(2);
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();
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
    constraints: new { controller = "^(Payment|Booking|AdminBooking|AdminCustomer|AdminMenu|OrderManagement|Settings|Table|Test|TestData)$" });

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

// Seed initial data (bao gồm đơn hàng demo cho dashboard)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<QLNHWebApp.Services.DataSeederService>();
    await seeder.SeedAsync();
}

app.Run();
