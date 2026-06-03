namespace BjuApiServer.Models
{
    public class GenerateCustomPlanRequest
    {
        public int UserId { get; set; }
        
        /// <summary>
        /// Опціональний список наявних продуктів, які AI має враховувати при генерації плану
        /// </summary>
        public List<string>? AvailableProducts { get; set; }

        /// <summary>
        /// Побажання користувача щодо рецептів (напр. "без цибулі", "тільки варене")
        /// </summary>
        public string? Preferences { get; set; }
    }
}