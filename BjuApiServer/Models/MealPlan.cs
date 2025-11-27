using System;

namespace BjuApiServer.Models
{
    public class MealPlan
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Текст плану харчування
        public string Plan { get; set; } = string.Empty;

        // Дата, на яку цей план згенеровано
        public DateTime Date { get; set; }
    }
}
