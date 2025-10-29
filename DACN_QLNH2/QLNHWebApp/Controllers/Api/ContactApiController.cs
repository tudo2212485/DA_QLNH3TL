using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebApp.Models;

namespace QLNHWebApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactApiController : ControllerBase
    {
        private readonly RestaurantDbContext _context;

        public ContactApiController(RestaurantDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ContactMessage>> CreateContactMessage([FromBody] ContactMessageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contactMessage = new ContactMessage
            {
                Name = request.Name,
                Email = request.Email,
                Message = request.Message,
                Date = DateTime.Now
            };

            _context.ContactMessages.Add(contactMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContactMessage), new { id = contactMessage.Id }, contactMessage);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactMessage>> GetContactMessage(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
                return NotFound();

            return message;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactMessage>>> GetContactMessages()
        {
            return await _context.ContactMessages.ToListAsync();
        }
    }

    public class ContactMessageRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
