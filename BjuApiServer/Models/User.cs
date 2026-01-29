namespace BjuApiServer.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public double Height { get; set; }
    public double Weight { get; set; }
    public int Age { get; set; }
    public string Goal { get; set; } = string.Empty;
    public string ActivityLevel { get; set; } = "Normal";

    // Нові поля для налаштувань
    public string Theme { get; set; } = "light";
    public string Language { get; set; } = "uk";

    // Нове поле для аватара
    public int AvatarId { get; set; } = 1;

    // Нове поле для статі
    public string Gender { get; set; } = "male";
}