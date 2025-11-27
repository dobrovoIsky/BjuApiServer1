using BjuApiServer.Data;
using BjuApiServer.Models;
using BjuApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // <--- ОСЬ ЦЕЙ РЯДОК ПОТРІБНО ДОДАТИ

namespace BjuApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NutritionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly BjuCalculationService _bjuService;
        private readonly GeminiService _geminiService;
        private readonly ILogger<NutritionController> _logger;

        public NutritionController(
            AppDbContext context,
            BjuCalculationService bjuService,
            GeminiService geminiService,
            ILogger<NutritionController> logger)
        {
            _context = context;
            _bjuService = bjuService;
            _geminiService = geminiService;
            _logger = logger;
        }

        [HttpPost("generate-custom-plan")]
        public async Task<IActionResult> GenerateCustomPlan([FromBody] GenerateCustomPlanRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {request.UserId} not found.");
            }

            var bjuResult = _bjuService.Calculate(user);

            var prompt = $"Створи детальний план харчування на один день для людини з такими цілями та показниками:" +
                         $"\n- Мета: {user.Goal}" +
                         $"\n- Денна норма калорій: {bjuResult.Calories} ккал" +
                         $"\n- Білки: {bjuResult.Proteins} г" +
                         $"\n- Жири: {bjuResult.Fats} г" +
                         $"\n- Вуглеводи: {bjuResult.Carbs} г" +
                         $"\n\nПлан має включати 3 основні прийоми їжі (сніданок, обід, вечеря) та 1-2 перекуси." +
                         $"Для кожної страви вкажи приблизний розмір порції в грамах та її калорійність. Відповідь надай українською мовою у форматі Markdown.";

            _logger.LogInformation("Generating custom plan for user {UserId}", user.Id);

            try
            {
                var mealPlanText = await _geminiService.GenerateMealPlanAsync(prompt);

                var newMealPlan = new MealPlan
                {
                    UserId = user.Id,
                    Plan = mealPlanText,
                    Date = DateTime.UtcNow
                };
                _context.MealPlans.Add(newMealPlan);
                await _context.SaveChangesAsync();

                return Ok(new { MealPlan = mealPlanText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate custom meal plan for user {UserId}", user.Id);
                return StatusCode(500, "An internal error occurred while generating the meal plan.");
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(int userId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                return NotFound($"User with ID {userId} not found.");
            }

            var history = await _context.MealPlans
                                        .Where(p => p.UserId == userId)
                                        .OrderByDescending(p => p.Date)
                                        .ToListAsync();

            return Ok(history);
        }
    }
}