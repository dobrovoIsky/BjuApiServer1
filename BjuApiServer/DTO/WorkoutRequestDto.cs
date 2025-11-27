namespace BjuApiServer.DTO
{
    /// <summary>
    /// Запит на генерацію плану тренувань для користувача.
    /// </summary>
    public class WorkoutRequestDto
    {
        public int UserId { get; set; }

        /// <summary>
        /// Ціль тренувань (наприклад: "схуднення", "набір маси", "витривалість").
        /// </summary>
        public string Goal { get; set; } = string.Empty;

        /// <summary>
        /// Інтенсивність: low / medium / high.
        /// </summary>
        public string Intensity { get; set; } = "medium";

        /// <summary>
        /// Скільки хвилин на день користувач готовий тренуватись.
        /// </summary>
        public int DurationMinutes { get; set; } = 45;
    }
}
