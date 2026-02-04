using BjuApiServer.Data;
using BjuApiServer.DTO;
using BjuApiServer.Models;
using BjuApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BjuApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeminiService _geminiService;
        private readonly ILogger<WorkoutsController> _logger;

        public WorkoutsController(
            AppDbContext context,
            GeminiService geminiService,
            ILogger<WorkoutsController> logger)
        {
            _context = context;
            _geminiService = geminiService;
            _logger = logger;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateWorkout([FromBody] WorkoutRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {request.UserId} not found.");
            }

            // Новий промпт для JSON на 1 день
            var prompt = $@"
Ти — професійний фітнес-тренер. Склади план тренування на ОДИН ДЕНЬ українською мовою.

ПРОФІЛЬ КОРИСТУВАЧА:
- Вік: {user.Age} років
- Зріст: {user.Height} см
- Вага: {user.Weight} кг
- Загальна ціль: {user.Goal}
- Рівень активності: {user.ActivityLevel}
- Стать: {user.Gender}

ПАРАМЕТРИ ТРЕНУВАННЯ:
- Тип тренування: {request.Goal}
- Інтенсивність: {request.Intensity}
- Тривалість: {request.DurationMinutes} хвилин

ВИМОГИ ДО ВІДПОВІДІ:
Поверни ЛИШЕ JSON об'єкт (без Markdown, без ```json) за такою схемою:
{{
  ""summary"": ""Короткий опис тренування (1 речення)"",
  ""warmup"": {{
    ""duration"": 5,
    ""exercises"": [""Вправа 1"", ""Вправа 2"", ""Вправа 3""]
  }},
  ""workout"": [
    {{
      ""name"": ""Назва вправи"",
      ""sets"": 3,
      ""reps"": ""12"",
      ""rest"": ""60 сек"",
      ""tips"": ""Короткі поради з техніки""
    }}
  ],
  ""cooldown"": {{
    ""duration"": 5,
    ""exercises"": [""Розтяжка 1"", ""Розтяжка 2""]
  }},
  ""totalCalories"": 300
}}

Створи 5-8 вправ в залежності від тривалості. Враховуй тип тренування:
- Якщо кардіо: біг, скакалка, берпі, велосипед тощо
- Якщо силове в залі: жим, присідання, тяга, тренажери
- Якщо вдома: планка, віджимання, присідання, випади

Назви вправ українською!";

            _logger.LogInformation("Generating workout plan for user {UserId}", user.Id);

            try
            {
                var workoutJson = await _geminiService.GenerateMealPlanAsync(prompt);

                var workoutPlan = new WorkoutPlan
                {
                    UserId = user.Id,
                    Goal = request.Goal,
                    Intensity = request.Intensity,
                    DurationMinutes = request.DurationMinutes,
                    PlanText = workoutJson,
                    CreatedAt = DateTime.UtcNow
                };

                _context.WorkoutPlans.Add(workoutPlan);
                await _context.SaveChangesAsync();

                // Повертаємо DTO
                var responseDto = new WorkoutPlanDto
                {
                    Id = workoutPlan.Id,
                    Goal = workoutPlan.Goal,
                    Intensity = workoutPlan.Intensity,
                    DurationMinutes = workoutPlan.DurationMinutes,
                    PlanText = workoutPlan.PlanText,
                    CreatedAt = workoutPlan.CreatedAt
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate workout plan for user {UserId}", user.Id);
                return StatusCode(500, "An internal error occurred while generating the workout plan.");
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(int userId)
        {
            var history = await _context.WorkoutPlans
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .Select(w => new WorkoutPlanDto
                {
                    Id = w.Id,
                    Goal = w.Goal,
                    Intensity = w.Intensity,
                    DurationMinutes = w.DurationMinutes,
                    PlanText = w.PlanText,
                    CreatedAt = w.CreatedAt
                })
                .ToListAsync();

            return Ok(history);
        }
    }
}