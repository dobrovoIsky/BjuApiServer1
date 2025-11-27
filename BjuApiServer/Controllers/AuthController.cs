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
        private readonly AppDbContext _context; // Використовуємо контекст БД
        private readonly ILogger<AuthController> _logger;

        // Інжектуємо AppDbContext замість JsonDbService
        public AuthController(AppDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegDTO userDto)
        {
            // ... (валідація вхідних даних залишається без змін)

            // Пошук користувача тепер відбувається в базі даних
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return BadRequest("Користувач з таким логіном вже існує.");
            }

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash),
                Height = userDto.Height,
                Weight = userDto.Weight,
                Age = userDto.Age,
                Goal = userDto.Goal,
                ActivityLevel = userDto.ActivityLevel,
                Theme = "light",
                Language = "uk"
            };

            // Додавання та збереження користувача в базі даних
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {Username} registered successfully with ID {UserId}", user.Username, user.Id);
            return Ok(new { message = "User registered successfully", userId = user.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDto)
        {
            _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);

            // Пошук користувача для входу також відбувається в базі даних
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            // Перевірка хешу пароля
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.PasswordHash, user.PasswordHash))
            {
                _logger.LogWarning("Login failed for user: {Username}. Invalid credentials.", loginDto.Username);
                return Unauthorized("Неправильний логін або пароль.");
            }

            _logger.LogInformation("User {Username} logged in successfully.", user.Username);
            return Ok(new { message = "Login successful", userId = user.Id });
        }
    }
}