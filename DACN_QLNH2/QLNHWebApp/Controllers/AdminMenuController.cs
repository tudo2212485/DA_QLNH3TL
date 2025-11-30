using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers
{
    [Authorize(AuthenticationSchemes = "AdminAuth", Policy = "AllRoles")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminMenuController : Controller
    {
        private readonly RestaurantDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminMenuController(RestaurantDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index(string searchString, string category)
        {
            var menuItems = _context.MenuItems.AsQueryable();

            // Case-insensitive search
            if (!string.IsNullOrEmpty(searchString))
            {
                menuItems = menuItems.Where(m =>
                    m.Name.ToLower().Contains(searchString.ToLower()) ||
                    m.Description.ToLower().Contains(searchString.ToLower()));
            }

            // Category filter
            if (!string.IsNullOrEmpty(category))
            {
                menuItems = menuItems.Where(m => m.Category == category);
            }

            var result = await menuItems
                .OrderBy(m => m.Category)
                .ThenBy(m => m.Name)
                .ToListAsync();

            // Pass search parameters to view
            ViewData["CurrentFilter"] = searchString;
            ViewData["CategoryFilter"] = category;

            return View(result);
        }

        [Authorize(Policy = "AdminAndStaff")] // Chỉ Admin và Nhân viên mới được tạo
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminAndStaff")]
        public async Task<IActionResult> Create(MenuItem menuItem, IFormFile? imageFile)
        {
            try
            {
                // Log thông tin để debug
                Console.WriteLine($"Creating menu item: {menuItem.Name}, Price: {menuItem.Price}, Category: {menuItem.Category}");

                // Validate image file if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    Console.WriteLine($"Image file: {imageFile.FileName}, Size: {imageFile.Length}");

                    // Check file size (1MB = 1048576 bytes)
                    if (imageFile.Length > 1048576)
                    {
                        ModelState.AddModelError("imageFile", "Dung lượng file không được vượt quá 1 MB");
                    }

                    // Check file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("imageFile", "Chỉ chấp nhận file JPEG, PNG");
                    }
                }

                // Log ModelState errors
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid:");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                    return View(menuItem);
                }

                // Handle image upload
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsPath = Path.Combine(_environment.WebRootPath, "images", "menu");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    menuItem.ImageUrl = $"/images/menu/{fileName}";
                    Console.WriteLine($"Image saved to: {menuItem.ImageUrl}");
                }

                _context.MenuItems.Add(menuItem);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Menu item saved successfully with ID: {menuItem.Id}");
                TempData["SuccessMessage"] = "Món ăn đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating menu item: {ex.Message}");
                TempData["ErrorMessage"] = $"Lỗi khi thêm món ăn: {ex.Message}";
                return View(menuItem);
            }
        }

        [Authorize(Policy = "AdminAndStaff")] // Chỉ Admin và Nhân viên mới được sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminAndStaff")]
        public async Task<IActionResult> Edit(int id, MenuItem menuItem, IFormFile? imageFile)
        {
            if (id != menuItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle image upload
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsPath = Path.Combine(_environment.WebRootPath, "images", "menu");
                        Directory.CreateDirectory(uploadsPath);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(_environment.WebRootPath, menuItem.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        menuItem.ImageUrl = $"/images/menu/{fileName}";
                    }

                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Món ăn đã được cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menuItem);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        [Authorize(Policy = "AdminAndStaff")] // Chỉ Admin và Nhân viên mới được xóa
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminAndStaff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(menuItem.ImageUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, menuItem.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Món ăn đã được xóa thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDetailedDescriptions()
        {
            var descriptions = new Dictionary<string, string>
            {
                ["Gỏi cuốn tôm thịt"] = "Gỏi cuốn tôm thịt là món khai vị truyền thống của Việt Nam, được làm từ tôm tươi, thịt ba chỉ luộc, bún tươi, rau thơm (rau răm, rau thái, xà lách) được cuốn trong bánh tráng mỏng. Món ăn có hương vị thanh mát, tươi ngon với nước chấm chua ngọt đặc trưng làm từ tương ớt, đường phèn, nước mắm và tỏi.",

                ["Bò lúc lắc"] = "Bò lúc lắc là món ăn đặc sản với thịt bò thăn được cắt miếng vuông vừa ăn, ướp gia vị đậm đà rồi xào nhanh trên chảo gang nóng. Thịt bò mềm ngọt, thơm phức, ăn kèm với bánh mì nướng giòn tan và salad rau tươi mát. Đây là sự kết hợp hoàn hảo giữa phong cách ẩm thực Âu và Á.",

                ["Sườn nướng mật ong"] = "Sườn nướng mật ong với những miếng sườn heo tươi ngon được ướp trong hỗn hợp mật ong tự nhiên, nước tương, tỏi băm và các gia vị đặc biệt. Sau khi nướng trên than hồng, sườn có vị ngọt đậm đà của mật ong, thịt mềm ngọt, da giòn rụm. Món ăn được phục vụ kèm cơm nóng và rau sống.",

                ["Lẩu thái hải sản"] = "Lẩu thái hải sản là món ăn đậm đà với nước dùng chua cay đặc trưng được nấu từ me, cà chua, ớt, sả, riềng và các loại gia vị Thái Lan. Lẩu có đầy đủ hải sản tươi sống như tôm, mực, cua, cá, nấm các loại và rau củ tươi ngon. Hương vị chua cay kích thích vị giác, phù hợp cho những buổi tụ tập gia đình.",

                ["Chè khúc bạch"] = "Chè khúc bạch là món tráng miệng mát lạnh truyền thống với lớp bánh flan mềm mịn, nước cốt dừa béo ngậy, và topping đa dạng như trân châu đen dai giòn, thạch dừa trong suốt, đậu đỏ ngọt bùi. Tất cả được phục vụ trong ly cao với đá bào mát lạnh, tạo nên món tráng miệng hoàn hảo cho ngày hè.",

                ["Trà đào cam sả"] = "Trà đào cam sả là thức uống giải khát tự nhiên với hương vị thanh mát đặc biệt. Được pha chế từ trà xanh thơm nhẹ, nước đào ngọt mát, cam tươi chua ngọt và sả thơm nồng tạo nên hương vị độc đáo. Thức uống được phục vụ với đá viên và lát cam tươi trang trí, rất phù hợp cho mọi thời tiết."
            };

            foreach (var desc in descriptions)
            {
                var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Name == desc.Key);
                if (menuItem != null)
                {
                    menuItem.Description = desc.Value;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật mô tả chi tiết cho tất cả món ăn.";
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItems.Any(e => e.Id == id);
        }
    }
}