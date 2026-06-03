using BjuApiServer.Data;
using BjuApiServer.Models;
using BjuApiServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            if (user == null) return NotFound("User not found.");

            // 1. Рахуємо математику
            var bju = _bjuService.Calculate(user);

            // 2. Формуємо частину промпту для наявних продуктів
            var availableProductsSection = "";
            if (request.AvailableProducts != null && request.AvailableProducts.Count > 0)
            {
                var productsList = string.Join(", ", request.AvailableProducts);
                availableProductsSection = $@"

            НАЯВНІ ПРОДУКТИ У КОРИСТУВАЧА:
            {productsList}
            
            ВАЖЛИВО: Запропонуй рецепти ПЕРЕВАЖНО з цих продуктів! Якщо потрібні додаткові базові інгредієнти (олія, сіль, спеції) - використовуй їх мінімально.";
            }

            var preferencesSection = "";
            if (!string.IsNullOrWhiteSpace(request.Preferences))
            {
                preferencesSection = $@"

            ОСОБЛИВІ ПОБАЖАННЯ КОРИСТУВАЧА (ВРАХУЙ ОБОВ'ЯЗКОВО):
            {request.Preferences}";
            }

            // 3. Формуємо "Інженерний" промпт для JSON
            var prompt = $@"
            Ти — професійний шеф-кухар та дієтолог. Твоє завдання — згенерувати кілька рецептів (3-4 варіанти) повноцінних страв у форматі JSON.
            
            ПРОФІЛЬ КОРИСТУВАЧА (для розуміння його цілей та розміру порції):
            - Стать: {user.Gender}
            - Ціль: {user.Goal}
            - Добова норма калорій: {bju.Calories} ккал
            {availableProductsSection}
            {preferencesSection}

            ВИМОГИ ДО ВІДПОВІДІ:
            Поверни ЛИШЕ JSON об'єкт (без Markdown, без ```json) за такою схемою:
            {{
              ""summary"": ""Короткий коментар дієтолога щодо цих рецептів (1 речення)"",
              ""meals"": [
                {{
                  ""name"": ""Назва страви/рецепту (наприклад: Салат з куркою, Яєчня з томатами)"",
                  ""time"": ""Опис процесу приготування (2-3 речення). Крок за кроком."",
                  ""foods"": [
                    {{ ""name"": ""Продукт"", ""weight"": ""грами/шт"", ""calories"": 100, ""protein"": 5, ""fat"": 2, ""carbs"": 10 }}
                  ],
                  ""totalCalories"": 0
                }}
              ]
            }}
            ВАЖЛИВО: 
            1. Використовуй поле 'time' для збереження ОПИСУ ПРОЦЕСУ ПРИГОТУВАННЯ.
            2. Назви страв і опис мають бути українською мовою.
            3. Запропонуй 3-4 різних цікавих рецепти з наявних продуктів.";

            _logger.LogInformation("Generating JSON plan for user {UserId}", user.Id);

            try
            {
                // 3. Отримуємо чистий JSON
                var jsonPlan = await _geminiService.GenerateMealPlanAsync(prompt);

                // 4. Зберігаємо в БД як рядок (клієнт розпарсить)
                var newMealPlan = new MealPlan
                {
                    UserId = user.Id,
                    Plan = jsonPlan, // Тут тепер лежить JSON, а не текст
                    Date = DateTime.UtcNow
                };

                _context.MealPlans.Add(newMealPlan);
                await _context.SaveChangesAsync();

                // Повертаємо клієнту теж JSON
                return Content(jsonPlan, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating plan.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(int userId)
        {
            var history = await _context.MealPlans
                                        .Where(p => p.UserId == userId)
                                        .OrderByDescending(p => p.Date)
                                        .ToListAsync();
            return Ok(history);
        }

        [HttpPut("history/{planId}/favorite")]
        public async Task<IActionResult> ToggleFavorite(int planId)
        {
            var plan = await _context.MealPlans.FindAsync(planId);
            if (plan == null) return NotFound("Plan not found.");

            plan.IsFavorite = !plan.IsFavorite;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Favorite status updated", isFavorite = plan.IsFavorite });
        }

        [HttpPost("analyze-image")]
        public async Task<IActionResult> AnalyzeImage([FromBody] AnalyzeImageRequest request)
        {
            if (string.IsNullOrEmpty(request?.Base64Image))
                return BadRequest("No image provided.");

            try
            {
                var jsonResult = await _geminiService.AnalyzeFoodImageAsync(request.Base64Image);
                return Content(jsonResult, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing image.");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}