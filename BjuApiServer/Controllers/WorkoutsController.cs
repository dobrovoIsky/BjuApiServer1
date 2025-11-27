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

        /// <summary>
        /// Генерує план тренувань для користувача з використанням Gemini
        /// і зберігає його в базу.
        /// </summary>
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

            // Формуємо промпт для Gemini
            var prompt =
                $"Склади детальний план тренувань українською мовою для користувача " +
                $"з такими характеристиками:" +
                $"\n- Вік: {user.Age} років" +
                $"\n- Зріст: {user.Height} см" +
                $"\n- Вага: {user.Weight} кг" +
                $"\n- Ціль користувача: {user.Goal}" +
                $"\n- Рівень активності: {user.ActivityLevel}" +
                $"\n\nДодаткові параметри тренування:" +
                $"\n- Ціль тренувань: {request.Goal}" +
                $"\n- Інтенсивність: {request.Intensity}" +
                $"\n- Тривалість: {request.DurationMinutes} хвилин на день." +
                "\n\nСклади план на 7 днів (або тижневий цикл), " +
                "розбий по днях, вкажи вправи, підходи, повторення, " +
                "рекомендації з розминки та заминки. Оформи відповідь у читабельному вигляді.";

            _logger.LogInformation("Generating workout plan for user {UserId}", user.Id);

            try
            {
                // Використовуємо вже існуючий метод для Gemini
                var workoutText = await _geminiService.GenerateMealPlanAsync(prompt);

                var workoutPlan = new WorkoutPlan
                {
                    UserId = user.Id,
                    Goal = request.Goal,
                    Intensity = request.Intensity,
                    DurationMinutes = request.DurationMinutes,
                    PlanText = workoutText,
                    CreatedAt = DateTime.UtcNow
                };

                _context.WorkoutPlans.Add(workoutPlan);
                await _context.SaveChangesAsync();

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

        /// <summary>
        /// Повертає історію планів тренувань користувача.
        /// </summary>
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetUserWorkouts(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var plans = await _context.WorkoutPlans
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            var result = plans.Select(w => new WorkoutPlanDto
            {
                Id = w.Id,
                Goal = w.Goal,
                Intensity = w.Intensity,
                DurationMinutes = w.DurationMinutes,
                PlanText = w.PlanText,
                CreatedAt = w.CreatedAt
            });

            return Ok(result);
        }
    }
}
