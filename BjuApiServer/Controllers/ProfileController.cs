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
                Bju = bjuResult,
                AvatarBase64 = user.AvatarBase64,
                Gender = user.Gender,
                Theme = user.Theme,
                Language = user.Language,
                Balance = user.Balance,
                MonthlyPoints = user.MonthlyPoints,
                CurrentStreak = user.CurrentStreak
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
            user.Gender = updateUserDto.Gender;

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
                Bju = bjuResult,
                AvatarBase64 = user.AvatarBase64,
                Gender = user.Gender,
                Theme = user.Theme,
                Language = user.Language,
                Balance = user.Balance,
                MonthlyPoints = user.MonthlyPoints,
                CurrentStreak = user.CurrentStreak
            };
            return Ok(userProfile);
        }

        [HttpPut("{id}/avatar")]
        public async Task<IActionResult> UpdateUserAvatar(int id, [FromBody] UpdateAvatarDto updateAvatarDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.AvatarBase64 = updateAvatarDto.AvatarBase64;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Avatar updated successfully.", avatarBase64 = user.AvatarBase64 });
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

        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.MonthlyPoints)
                .Take(50)
                .Select(u => new LeaderboardUserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    AvatarBase64 = u.AvatarBase64,
                    Points = u.MonthlyPoints,
                    CurrentStreak = u.CurrentStreak
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("{id}/check-streak")]
        public async Task<IActionResult> CheckStreak(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            var today = DateTime.UtcNow.Date;
            var nextDay = today.AddDays(1);

            // Monthly Reset Check
            if (user.LastMonthlyReset == null || user.LastMonthlyReset.Value.Month != today.Month || user.LastMonthlyReset.Value.Year != today.Year)
            {
                user.MonthlyPoints = 0;
                user.LastMonthlyReset = today;
            }

            // Get total calories for today
            var totalCaloriesToday = await _context.FoodEntries
                .Where(f => f.UserId == id && f.LoggedAt >= today && f.LoggedAt < nextDay)
                .SumAsync(f => f.Calories);

            var bjuGoal = _bjuService.Calculate(user);
            var isGoalReached = totalCaloriesToday >= (bjuGoal.Calories * 0.8) && totalCaloriesToday <= (bjuGoal.Calories * 1.15); // goal reached if between 80% and 115%

            if (isGoalReached)
            {
                if (user.LastGoalReachedDate == null || user.LastGoalReachedDate.Value.Date < today)
                {
                    // Update streak
                    if (user.LastGoalReachedDate != null && user.LastGoalReachedDate.Value.Date == today.AddDays(-1))
                    {
                        user.CurrentStreak++;
                    }
                    else
                    {
                        user.CurrentStreak = 1; // Restart streak
                    }
                    
                    user.LastGoalReachedDate = today;
                    int pointsEarned = 10 + (user.CurrentStreak * 2); // points formula based on streak
                    user.Balance += pointsEarned;
                    user.MonthlyPoints += pointsEarned;
                    await _context.SaveChangesAsync();

                    return Ok(new { StreakUpdated = true, CurrentStreak = user.CurrentStreak, PointsEarned = pointsEarned, TotalPoints = user.Balance, MonthlyPoints = user.MonthlyPoints });
                }
            }
            else
            {
                // Check if they broke their streak
                if (user.LastGoalReachedDate != null && user.LastGoalReachedDate.Value.Date < today.AddDays(-1))
                {
                    // They missed yesterday
                    user.CurrentStreak = 0;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { StreakUpdated = false, CurrentStreak = user.CurrentStreak, TotalPoints = user.Balance, MonthlyPoints = user.MonthlyPoints });
        }
    }
}