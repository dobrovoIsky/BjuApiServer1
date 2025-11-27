using BjuApiServer.Models;

namespace BjuApiServer.Services
{
    public class BjuCalculationService
    {
        public BjuResult Calculate(User user)
        {
            // Розрахунок базового метаболізму (BMR) за формулою Міффліна-Сан-Жеора
            // Для простоти припустимо, що користувач - чоловік. У реальному додатку потрібно додати поле "стать".
            double bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;

            // Розрахунок денної норми калорій (TDEE) з урахуванням активності
            double tdee = bmr * GetActivityMultiplier(user.ActivityLevel);

            // Корекція калорій в залежності від мети
            double targetCalories = tdee + GetGoalModifier(user.Goal);

            // Розрахунок БЖУ в грамах
            // (1г білка = 4 ккал, 1г жиру = 9 ккал, 1г вуглеводу = 4 ккал)
            double proteins, fats, carbs;

            switch (user.Goal.ToLower())
            {
                case "gain muscle": // Набір маси
                case "набір маси":
                    proteins = (targetCalories * 0.30) / 4;
                    fats = (targetCalories * 0.30) / 9;
                    carbs = (targetCalories * 0.40) / 4;
                    break;

                case "maintain weight": // Підтримка ваги
                case "підтримка ваги":
                    proteins = (targetCalories * 0.25) / 4;
                    fats = (targetCalories * 0.30) / 9;
                    carbs = (targetCalories * 0.45) / 4;
                    break;

                case "lose weight": // Схуднення
                case "схуднення":
                default:
                    proteins = (targetCalories * 0.40) / 4;
                    fats = (targetCalories * 0.30) / 9;
                    carbs = (targetCalories * 0.30) / 4;
                    break;
            }

            return new BjuResult
            {
                Calories = Math.Round(targetCalories),
                Proteins = Math.Round(proteins),
                Fats = Math.Round(fats),
                Carbs = Math.Round(carbs)
            };
        }

        private double GetActivityMultiplier(string activityLevel)
        {
            return activityLevel.ToLower() switch
            {
                "sedentary" or "сидячий" => 1.2,
                "lightly active" or "легка активність" => 1.375,
                "moderately active" or "помірна активність" => 1.55,
                "very active" or "висока активність" => 1.725,
                _ => 1.2
            };
        }

        private int GetGoalModifier(string goal)
        {
            return goal.ToLower() switch
            {
                "gain muscle" or "набір маси" => 300,  // Профіцит 300 ккал
                "lose weight" or "схуднення" => -300, // Дефіцит 300 ккал
                _ => 0
            };
        }
    }
}