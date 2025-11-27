namespace BjuApiServer.Models
{
    public class WorkoutPlan
    {
        public int Id { get; set; }

        // Кому належить план
        public int UserId { get; set; }

        // Напр. "схуднення", "набір маси", "витривалість"
        public string Goal { get; set; } = string.Empty;

        // Рівень інтенсивності: low / medium / high
        public string Intensity { get; set; } = "medium";

        // Тривалість в хвилинах на день
        public int DurationMinutes { get; set; }

        // План, який повернув Gemini (текст)
        public string PlanText { get; set; } = string.Empty;

        // Дата створення / рекомендації
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
