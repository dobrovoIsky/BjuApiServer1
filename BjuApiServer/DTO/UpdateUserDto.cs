namespace BjuApiServer.DTO
{
    // Цей клас містить тільки ті поля, які користувач може оновити
    public class UpdateUserDto
    {
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = string.Empty;
        public int AvatarId { get; set; } = 1;
        public string Gender { get; set; } = "male";
    }
}