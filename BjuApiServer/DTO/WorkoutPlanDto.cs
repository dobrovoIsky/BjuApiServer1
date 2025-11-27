using System;

namespace BjuApiServer.DTO
{
    /// <summary>
    /// DTO для повернення плану тренувань на клієнт.
    /// </summary>
    public class WorkoutPlanDto
    {
        public int Id { get; set; }

        public string Goal { get; set; } = string.Empty;

        public string Intensity { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public string PlanText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
