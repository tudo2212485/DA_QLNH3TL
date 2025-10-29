using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuApiController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public MenuApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItems(string? category = null, string? search = null)
        {
            var items = _context.MenuItems.AsQueryable();
            
            if (!string.IsNullOrEmpty(category))
                items = items.Where(m => m.Category == category);
            
            if (!string.IsNullOrEmpty(search))
                items = items.Where(m => m.Name.Contains(search));

            return await items.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
        {
            var item = await _context.MenuItems
                .Include(m => m.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
                return NotFound();

            return item;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var categories = await _context.MenuItems
                .Select(m => m.Category)
                .Distinct()
                .ToListAsync();
            
            return categories;
        }

        [HttpPost("{menuItemId}/ratings")]
        public async Task<IActionResult> AddRating(int menuItemId, [FromBody] RatingRequest request)
        {
            if (request.Score < 1 || request.Score > 5)
                return BadRequest("Rating must be between 1 and 5");

            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
                return NotFound();

            var rating = new Rating
            {
                MenuItemId = menuItemId,
                Score = request.Score,
                Comment = request.Comment ?? string.Empty,
                Date = DateTime.Now
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rating added successfully" });
        }
    }

    public class RatingRequest
    {
        public int Score { get; set; }
        public string? Comment { get; set; }
    }
}

