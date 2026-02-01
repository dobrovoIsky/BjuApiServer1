using BjuApiServer.Data;
using BjuApiServer.DTO;
using BjuApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BjuApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AppDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegDTO userDto)
        {
            if (string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.PasswordHash))
            {
                return BadRequest("Username and password are required.");
            }

            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return BadRequest("Користувач з таким логіном вже існує.");
            }

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.PasswordHash, // Простий пароль
                Height = userDto.Height,
                Weight = userDto.Weight,
                Age = userDto.Age,
                Goal = userDto.Goal,
                ActivityLevel = userDto.ActivityLevel,
                Theme = "light",
                Language = "uk",
                Gender = userDto.Gender,
                AvatarId = 1
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Username} registered successfully with ID {UserId}", user.Username, user.Id);
            return Ok(new { message = "User registered successfully", userId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || loginDto.PasswordHash != user.PasswordHash) // Просте порівняння
            {
                _logger.LogWarning("Login failed for user: {Username}. Invalid credentials.", loginDto.Username);
                return Unauthorized("Неправильний логін або пароль.");
            }

            _logger.LogInformation("User {Username} logged in successfully.", user.Username);
            return Ok(new { message = "Login successful", userId = user.Id });
        }

        [HttpPost("create-test-user")]
        public async Task<IActionResult> CreateTestUser()
        {
            if (await _context.Users.AnyAsync(u => u.Username == "testuser"))
            {
                return Ok("Test user already exists.");
            }

            var user = new User
            {
                Username = "testuser",
                PasswordHash = "testpass", // Простий пароль
                Height = 170,
                Weight = 70,
                Age = 25,
                Goal = "maintain weight",
                ActivityLevel = "moderately active",
                Theme = "light",
                Language = "uk",
                Gender = "male",
                AvatarId = 1
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("Test user created. Username: testuser, Password: testpass");
        }
    }
}