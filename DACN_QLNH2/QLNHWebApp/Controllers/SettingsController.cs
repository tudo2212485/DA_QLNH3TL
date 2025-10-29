using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminOnly")]
    [Route("Admin/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)] // Loại khỏi Swagger vì đây là MVC controller, không phải API
    public class SettingsController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SettingsController(RestaurantDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                settings = new RestaurantSettings
                {
                    RestaurantName = "Nhà Hàng 3TL",
                    Address = "123 Đường ABC, Quận XYZ, TP.HCM",
                    Phone = "0123-456-789",
                    Email = "info@3tlrestaurant.com",
                    OpenTime = TimeOnly.Parse("08:00"),
                    CloseTime = TimeOnly.Parse("22:00"),
                    TaxRate = 0.1M
                };
                _context.RestaurantSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGeneral(RestaurantSettings model, IFormFile? logoFile)
        {
            var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                return NotFound();
            }

            // Update basic info
            settings.RestaurantName = model.RestaurantName;
            settings.Address = model.Address;
            settings.Phone = model.Phone;
            settings.Email = model.Email;
            settings.OpenTime = model.OpenTime;
            settings.CloseTime = model.CloseTime;
            settings.TaxRate = model.TaxRate;

            // Handle logo upload
            if (logoFile != null && logoFile.Length > 0)
            {
                var uploadsPath = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsPath);

                var fileName = "logo" + Path.GetExtension(logoFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(fileStream);
                }

                // Logo đã được upload thành công
            }

            _context.Update(settings);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cài đặt đã được cập nhật thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  
        public async Task<IActionResult> UpdateSystem(RestaurantSettings model)
        {
            var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                return NotFound();
            }

            // Chỉ cập nhật các properties có trong model
            settings.TaxRate = model.TaxRate;

            _context.Update(settings);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cài đặt hệ thống đã được cập nhật!";
            return RedirectToAction(nameof(Index));
        }

        // API endpoint for getting settings
        [HttpGet("api")]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();
            return Json(settings);
        }

        // Reset to default settings
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetToDefault()
        {
            var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();
            if (settings != null)
            {
                settings.RestaurantName = "Nhà Hàng 3TL";
                settings.Address = "123 Đường ABC, Quận XYZ, TP.HCM";
                settings.Phone = "0123-456-789";
                settings.Email = "info@3tlrestaurant.com";
                settings.OpenTime = TimeOnly.Parse("08:00");
                settings.CloseTime = TimeOnly.Parse("22:00");
                settings.TaxRate = 0.1M;

                _context.Update(settings);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Đã khôi phục cài đặt mặc định!";
            return RedirectToAction(nameof(Index));
        }
    }
}