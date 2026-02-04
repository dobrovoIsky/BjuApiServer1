using BjuApiServer.Data;
using BjuApiServer.DTO;
using BjuApiServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using System.Text.Json;
using System.Security.Cryptography;

namespace BjuApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(
            AppDbContext context,
            ILogger<AuthController> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
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

            if (!string.IsNullOrWhiteSpace(userDto.Email))
            {
                if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
                {
                    return BadRequest("Користувач з такою поштою вже існує.");
                }
            }

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.PasswordHash,
                Email = userDto.Email ?? string.Empty,
                Height = userDto.Height,
                Weight = userDto.Weight,
                Age = userDto.Age,
                Goal = userDto.Goal,
                ActivityLevel = userDto.ActivityLevel,
                Theme = "light",
                Language = "uk",
                Gender = userDto.Gender,
                AvatarId = 1,
                IsGoogleUser = false
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

            if (user == null || loginDto.PasswordHash != user.PasswordHash)
            {
                _logger.LogWarning("Login failed for user: {Username}. Invalid credentials.", loginDto.Username);
                return Unauthorized("Неправильний логін або пароль.");
            }

            // Генеруємо токен
            var token = GenerateSecureToken();

            _logger.LogInformation("User {Username} logged in successfully.", user.Username);
            return Ok(new { message = "Login successful", userId = user.Id, token = token });
        }

        // ===== GOOGLE CALLBACK - редірект назад у додаток =====
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                _logger.LogInformation("Google callback received");

                var clientId = _configuration["Google:ClientId"];
                var clientSecret = _configuration["Google:ClientSecret"];
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google-callback";

                // Обмінюємо code на tokens
                var httpClient = _httpClientFactory.CreateClient();
                var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["code"] = code,
                        ["client_id"] = clientId,
                        ["client_secret"] = clientSecret,
                        ["redirect_uri"] = redirectUri,
                        ["grant_type"] = "authorization_code"
                    }));

                var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get token: {Response}", tokenJson);
                    return RedirectToDeepLink(0, "", $"Помилка: {tokenJson}");
                }

                var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenJson);
                var idToken = tokenData.GetProperty("id_token").GetString();

                // Верифікуємо id_token
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                });

                _logger.LogInformation("Google token validated for email: {Email}", payload.Email);

                // Шукаємо або створюємо користувача
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.GoogleId == payload.Subject || u.Email == payload.Email);

                if (user == null)
                {
                    user = new User
                    {
                        Username = payload.Name ?? payload.Email.Split('@')[0],
                        Email = payload.Email,
                        PasswordHash = Guid.NewGuid().ToString(),
                        GoogleId = payload.Subject,
                        IsGoogleUser = true,
                        Height = 170,
                        Weight = 70,
                        Age = 25,
                        Goal = "maintain weight",
                        ActivityLevel = "moderately active",
                        Gender = "male",
                        AvatarId = 1
                    };

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("New Google user created: {Email}, ID: {UserId}", user.Email, user.Id);
                }
                else if (string.IsNullOrEmpty(user.GoogleId))
                {
                    user.GoogleId = payload.Subject;
                    user.IsGoogleUser = true;
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        user.Email = payload.Email;
                    }
                    await _context.SaveChangesAsync();
                }

                // Генеруємо токен
                var authToken = GenerateSecureToken();

                // Редіректимо назад у додаток через Deep Link
                return RedirectToDeepLink(user.Id, authToken, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google callback error");
                return RedirectToDeepLink(0, "", ex.Message);
            }
        }

        // ===== FORGOT PASSWORD =====
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return Ok(new { message = "Якщо email існує, ви отримаєте лист з інструкціями." });
            }

            var resetCode = new Random().Next(100000, 999999).ToString();
            _logger.LogInformation("Password reset code for {Email}: {Code}", dto.Email, resetCode);

            return Ok(new { message = "Якщо email існує, ви отримаєте лист з інструкціями." });
        }

        // ===== HELPERS =====

        private IActionResult RedirectToDeepLink(int userId, string token, string error)
        {
            string deepLink;

            if (!string.IsNullOrEmpty(error))
            {
                deepLink = $"nutritionapp://auth/callback?error={Uri.EscapeDataString(error)}";
            }
            else
            {
                deepLink = $"nutritionapp://auth/callback?userId={userId}&token={Uri.EscapeDataString(token)}";
            }

            _logger.LogInformation("Redirecting to deep link: {DeepLink}", deepLink);
            return Redirect(deepLink);
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }

    public class GoogleAuthDTO
    {
        public string IdToken { get; set; } = string.Empty;
    }

    public class ForgotPasswordDTO
    {
        public string Email { get; set; } = string.Empty;
    }
}