using BjuApiServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BjuApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly GeminiService _gemini;
    private readonly ILogger<AiController> _logger;

    public AiController(GeminiService gemini, ILogger<AiController> logger)
    {
        _gemini = gemini;
        _logger = logger;
    }

    [HttpPost("mealplan")]
    public async Task<IActionResult> MealPlan([FromBody] string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
            return BadRequest("Prompt is required.");

        try
        {
            var result = await _gemini.GenerateMealPlanAsync(prompt);
            return Ok(new { result });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Gemini API key is not configured.");
            return StatusCode(500, "Gemini API key is not configured.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Gemini API.");
            return StatusCode(500, "AI service error.");
        }
    }
}