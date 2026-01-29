using BjuApiServer.Models;

namespace BjuApiServer.Services
{
    public class BjuCalculationService
    {
        public BjuResult Calculate(User user)
        {
            // Розрахунок базового метаболізму (BMR) за формулою Mifflin-St Jeor
            double bmr;
            if (user.Gender.ToLower() == "male")
            {
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age + 5;
            }
            else
            {
                bmr = 10 * user.Weight + 6.25 * user.Height - 5 * user.Age - 161;
            }

            // Коефіцієнт активності
            double activityMultiplier = user.ActivityLevel.ToLower() switch
            {
                "sedentary" or "малорухливий" => 1.2,
                "lightly active" or "легка активність" => 1.375,
                "moderately active" or "помірна активність" => 1.55,
                "very active" or "дуже активний" => 1.725,
                _ => 1.2
            };

            double tdee = bmr * activityMultiplier;

            // Коефіцієнт для цілі (Goal)
            double goalMultiplier;
            string goal = user.Goal.ToLower();
            if (goal.Contains("lose") || goal.Contains("зниження") || goal.Contains("схуднути") || goal.Contains("схуднення"))
            {
                goalMultiplier = 0.8; // Дефіцит для схуднення
            }
            else if (goal.Contains("maintain") || goal.Contains("підтримка") || goal.Contains("підтримати") || goal.Contains("ваги"))
            {
                goalMultiplier = 1.0; // Підтримка
            }
            else if (goal.Contains("gain") || goal.Contains("набір") || goal.Contains("м'яз") || goal.Contains("маси"))
            {
                goalMultiplier = 1.2; // Надлишок для набору
            }
            else
            {
                goalMultiplier = 1.0; // За замовчуванням підтримка
            }

            double dailyCalories = tdee * goalMultiplier;

            // Розподіл БЖУ (приблизно: 30% білки, 50% вуглеводи, 20% жири)
            double proteinsGrams = dailyCalories * 0.3 / 4; // 4 кал на грам білка
            double carbsGrams = dailyCalories * 0.5 / 4;    // 4 кал на грам вуглеводів
            double fatsGrams = dailyCalories * 0.2 / 9;     // 9 кал на грам жиру

            return new BjuResult
            {
                Calories = dailyCalories,
                Proteins = proteinsGrams,
                Carbs = carbsGrams,
                Fats = fatsGrams
            };
        }
    }
}