using BjuApiServer.Models;

namespace BjuApiServer.Services
{
    public class BjuCalculationService
    {
        public BjuResult Calculate(User user)
        {
            // ЗАХИСТ: Якщо даних немає, ставимо дефолтні значення, щоб сервер не падав
            double weight = user.Weight > 0 ? user.Weight : 70;
            double height = user.Height > 0 ? user.Height : 170;
            int age = user.Age > 0 ? user.Age : 25;
            string activity = !string.IsNullOrEmpty(user.ActivityLevel) ? user.ActivityLevel : "sedentary";
            string goal = !string.IsNullOrEmpty(user.Goal) ? user.Goal : "maintain weight";

            // Розрахунок BMR (Mifflin-St Jeor)
            double bmr = 10 * weight + 6.25 * height - 5 * age + 5;

            // TDEE
            double tdee = bmr * GetActivityMultiplier(activity);

            // Goal correction
            double targetCalories = tdee + GetGoalModifier(goal);

            // Macros
            double proteins, fats, carbs;

            switch (goal.ToLower())
            {
                case "gain muscle":
                case "набір маси":
                    proteins = (targetCalories * 0.30) / 4;
                    fats = (targetCalories * 0.30) / 9;
                    carbs = (targetCalories * 0.40) / 4;
                    break;

                case "lose weight":
                case "схуднення":
                    proteins = (targetCalories * 0.40) / 4;
                    fats = (targetCalories * 0.30) / 9;
                    carbs = (targetCalories * 0.30) / 4;
                    break;

                case "maintain weight":
                case "підтримка ваги":
                default:
                    proteins = (targetCalories * 0.25) / 4;
                    fats = (targetCalories * 0.30) / 9;
                    carbs = (targetCalories * 0.45) / 4;
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
            if (string.IsNullOrEmpty(activityLevel)) return 1.2;

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
            if (string.IsNullOrEmpty(goal)) return 0;

            return goal.ToLower() switch
            {
                "gain muscle" or "набір маси" => 300,
                "lose weight" or "схуднення" => -300,
                _ => 0
            };
        }
    }
}