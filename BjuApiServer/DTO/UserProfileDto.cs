using BjuApiServer.Models;

namespace BjuApiServer.DTO
{
    // Цей клас об'єднує дані користувача та його розраховане БЖУ
    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = "Normal";
        public BjuResult Bju { get; set; }
        public string? AvatarBase64 { get; set; }
        public string Gender { get; set; } = "male";
        public string Theme { get; set; } = "light";
        public string Language { get; set; } = "uk";
        public int Balance { get; set; }
        public int MonthlyPoints { get; set; }
        public int CurrentStreak { get; set; }
    }
}