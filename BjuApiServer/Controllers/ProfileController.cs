using BjuApiServer.Data;
using BjuApiServer.DTO;
using BjuApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BjuApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BjuCalculationService _bjuService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(AppDbContext context, BjuCalculationService bjuService, ILogger<ProfileController> logger)
        {
            _context = context;
            _bjuService = bjuService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(int id)
        {
            _logger.LogInformation("Attempting to get profile for user with ID: {UserId}", id);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID: {UserId} not found.", id);
                return NotFound("User not found.");
            }

            var bjuResult = _bjuService.Calculate(user);
            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Height = user.Height,
                Weight = user.Weight,
                Age = user.Age,
                Goal = user.Goal,
                ActivityLevel = user.ActivityLevel,
                CalculatedBju = bjuResult,
                AvatarId = user.AvatarId // Додано
            };
            return Ok(userProfile);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.Height = updateUserDto.Height;
            user.Weight = updateUserDto.Weight;
            user.Age = updateUserDto.Age;
            user.Goal = updateUserDto.Goal;
            user.ActivityLevel = updateUserDto.ActivityLevel;
            user.AvatarId = updateUserDto.AvatarId; // Додано

            await _context.SaveChangesAsync();

            var bjuResult = _bjuService.Calculate(user);
            var userProfile = new UserProfileDto
            {
                Id = user.Id,
                Username = user.Username,
                Height = user.Height,
                Weight = user.Weight,
                Age = user.Age,
                Goal = user.Goal,
                ActivityLevel = user.ActivityLevel,
                CalculatedBju = bjuResult,
                AvatarId = user.AvatarId // Додано
            };
            return Ok(userProfile);
        }

        [HttpGet("{id}/settings")]
        public async Task<IActionResult> GetUserSettings(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            var settings = new SettingsDto
            {
                Theme = user.Theme,
                Language = user.Language
            };
            return Ok(settings);
        }

        [HttpPut("{id}/settings")]
        public async Task<IActionResult> UpdateUserSettings(int id, [FromBody] SettingsDto settingsDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.Theme = settingsDto.Theme ?? user.Theme;
            user.Language = settingsDto.Language ?? user.Language;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Settings updated successfully." });
        }
    }
}