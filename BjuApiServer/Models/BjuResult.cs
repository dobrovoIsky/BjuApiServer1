namespace BjuApiServer.Models
{
    // Цей клас буде зберігати результат розрахунків
    public class BjuResult
    {
        public double Calories { get; set; }
        public double Proteins { get; set; }
        public double Fats { get; set; }
        public double Carbs { get; set; }
    }
}