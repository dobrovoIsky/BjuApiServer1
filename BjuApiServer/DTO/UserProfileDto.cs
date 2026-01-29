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
        public string ActivityLevel { get; set; } = string.Empty;
        public BjuResult CalculatedBju { get; set; }
        public int AvatarId { get; set; } = 1;
        public string Gender { get; set; } = "male";
    }
}