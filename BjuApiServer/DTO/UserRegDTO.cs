namespace BjuApiServer.DTO
{
    public class UserRegDTO
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public string Goal { get; set; } = string.Empty;
        public string ActivityLevel { get; set; } = "Normal";
        public string Gender { get; set; } = "male";
    }
}