using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AdminOnly")]
    public class SettingsController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SettingsController(RestaurantDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("Admin/Settings")]
        [Route("Admin/Settings/Index")]
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

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("Admin/Settings/UpdateGeneral")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGeneral(RestaurantSettings model, IFormFile? logoFile)
        {
            try
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
                }

                _context.Update(settings);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cài đặt đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("Admin/Settings/UpdateSystem")]
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
        [HttpGet]
        [Route("api/settings")]
        [ProducesResponseType(typeof(RestaurantSettings), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();

            if (settings == null)
            {
                return NotFound(new { message = "Cài đặt không tồn tại" });
            }

            return Ok(settings);
        }

        // Reset to default settings
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("Admin/Settings/ResetToDefault")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetToDefault()
        {
            try
            {
                var settings = await _context.RestaurantSettings.FirstOrDefaultAsync();
                if (settings != null)
                {
                    settings.RestaurantName = "Nhà Hàng 3TL";
                    settings.Address = "123 Đường ABC, Quận 1, TP.HCM";
                    settings.Phone = "028-1234-5679";
                    settings.Email = "info@3tlrestaurant.com";
                    settings.OpenTime = TimeOnly.Parse("10:00");
                    settings.CloseTime = TimeOnly.Parse("22:00");
                    settings.TaxRate = 0.1M;

                    _context.Update(settings);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Đã khôi phục cài đặt mặc định!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi khôi phục: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}