using System.ComponentModel.DataAnnotations;

namespace BjuApiServer.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = "Normal";
        public string Theme { get; set; } = "light";
        public string Language { get; set; } = "uk";
        public string Gender { get; set; } = "male";
        public int AvatarId { get; set; } = 1;

        // Google Auth
        public string? GoogleId { get; set; }
        public bool IsGoogleUser { get; set; } = false;
    }
}