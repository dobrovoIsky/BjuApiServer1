namespace BjuApiServer.Models
{
    public class MealPlanRequest
    {
        // Надаємо значення за замовчуванням, щоб уникнути попередження
        public string Prompt { get; set; } = string.Empty;
    }
}