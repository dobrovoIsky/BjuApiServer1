using BjuApiServer.Data;
using BjuApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BjuApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TrackerController> _logger;

        public TrackerController(AppDbContext context, ILogger<TrackerController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogFood([FromBody] FoodEntry entry)
        {
            entry.LoggedAt = DateTime.UtcNow;
            _context.FoodEntries.Add(entry);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Food logged for user {UserId}: {Name} ({Calories} kcal)", entry.UserId, entry.Name, entry.Calories);
            return Ok(entry);
        }

        [HttpGet("daily/{userId}")]
        public async Task<IActionResult> GetDaily(int userId, [FromQuery] string? date = null)
        {
            DateTime targetDate = date != null ? DateTime.Parse(date).ToUniversalTime().Date : DateTime.UtcNow.Date;
            var nextDay = targetDate.AddDays(1);

            var entries = await _context.FoodEntries
                .Where(f => f.UserId == userId && f.LoggedAt >= targetDate && f.LoggedAt < nextDay)
                .OrderByDescending(f => f.LoggedAt)
                .ToListAsync();

            return Ok(entries);
        }

        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetDailySummary(int userId, [FromQuery] string? date = null)
        {
            DateTime targetDate = date != null ? DateTime.Parse(date).ToUniversalTime().Date : DateTime.UtcNow.Date;
            var nextDay = targetDate.AddDays(1);

            var entries = await _context.FoodEntries
                .Where(f => f.UserId == userId && f.LoggedAt >= targetDate && f.LoggedAt < nextDay)
                .ToListAsync();

            var summary = new
            {
                TotalCalories = entries.Sum(e => e.Calories),
                TotalProtein = entries.Sum(e => e.Protein),
                TotalFat = entries.Sum(e => e.Fat),
                TotalCarbs = entries.Sum(e => e.Carbs),
                Count = entries.Count
            };

            return Ok(summary);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            var entry = await _context.FoodEntries.FindAsync(id);
            if (entry == null) return NotFound();

            _context.FoodEntries.Remove(entry);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Deleted" });
        }
        // GET: /api/tracker/products
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            try
            {
                var products = await _context.ProductItems
                    .OrderBy(p => p.Name)
                    .ToListAsync();
                
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Помилка при отриманні продуктів", details = ex.Message });
            }
        }
    }
}