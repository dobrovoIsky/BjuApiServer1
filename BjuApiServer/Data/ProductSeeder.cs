using BjuApiServer.Models;
using BjuApiServer.Data;
using Microsoft.EntityFrameworkCore;

namespace BjuApiServer.Data
{
    public static class ProductSeeder
    {
        public static async Task SeedProductsAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Запускаємо міграції
            await context.Database.MigrateAsync();

            if (await context.ProductItems.AnyAsync())
            {
                return; // База вже заповнена
            }

            var products = new List<ProductItem>
            {
                // М'ясо та птиця
                new ProductItem { Name = "Куряче філе (сире)", CaloriesPer100g = 113, ProteinPer100g = 23.6, FatPer100g = 1.9, CarbsPer100g = 0.4 },
                new ProductItem { Name = "Куряче філе (відварене)", CaloriesPer100g = 153, ProteinPer100g = 30.4, FatPer100g = 3.5, CarbsPer100g = 0 },
                new ProductItem { Name = "Куряче стегно", CaloriesPer100g = 185, ProteinPer100g = 19.3, FatPer100g = 11.9, CarbsPer100g = 0 },
                new ProductItem { Name = "Індичка (філе)", CaloriesPer100g = 104, ProteinPer100g = 22.6, FatPer100g = 1.5, CarbsPer100g = 0 },
                new ProductItem { Name = "Свинина (пісна)", CaloriesPer100g = 143, ProteinPer100g = 21, FatPer100g = 6.5, CarbsPer100g = 0 },
                new ProductItem { Name = "Свинина (жирна)", CaloriesPer100g = 316, ProteinPer100g = 14.1, FatPer100g = 28.8, CarbsPer100g = 0 },
                new ProductItem { Name = "Яловичина (пісна)", CaloriesPer100g = 158, ProteinPer100g = 22.2, FatPer100g = 7.1, CarbsPer100g = 0 },
                new ProductItem { Name = "Сало", CaloriesPer100g = 797, ProteinPer100g = 2.4, FatPer100g = 89, CarbsPer100g = 0 },
                new ProductItem { Name = "Печінка куряча", CaloriesPer100g = 137, ProteinPer100g = 20.4, FatPer100g = 5.9, CarbsPer100g = 0.7 },
                new ProductItem { Name = "Ковбаса варена (Докторська)", CaloriesPer100g = 257, ProteinPer100g = 12.8, FatPer100g = 22.2, CarbsPer100g = 1.5 },
                new ProductItem { Name = "Ковбаса сирокопчена", CaloriesPer100g = 472, ProteinPer100g = 24.8, FatPer100g = 41.5, CarbsPer100g = 0 },
                new ProductItem { Name = "Сосиски", CaloriesPer100g = 266, ProteinPer100g = 10.4, FatPer100g = 24, CarbsPer100g = 2 },
                
                // Риба та морепродукти
                new ProductItem { Name = "Лосось (сирий)", CaloriesPer100g = 208, ProteinPer100g = 20.4, FatPer100g = 13.4, CarbsPer100g = 0 },
                new ProductItem { Name = "Сьомга (слабосолона)", CaloriesPer100g = 202, ProteinPer100g = 22.5, FatPer100g = 12.5, CarbsPer100g = 0 },
                new ProductItem { Name = "Хек", CaloriesPer100g = 86, ProteinPer100g = 16.6, FatPer100g = 2.2, CarbsPer100g = 0 },
                new ProductItem { Name = "Минтай", CaloriesPer100g = 72, ProteinPer100g = 15.9, FatPer100g = 0.9, CarbsPer100g = 0 },
                new ProductItem { Name = "Оселедець", CaloriesPer100g = 161, ProteinPer100g = 16.3, FatPer100g = 10.7, CarbsPer100g = 0 },
                new ProductItem { Name = "Скумбрія", CaloriesPer100g = 191, ProteinPer100g = 18, FatPer100g = 13.2, CarbsPer100g = 0 },
                new ProductItem { Name = "Тунець (консервований у власному соку)", CaloriesPer100g = 96, ProteinPer100g = 21, FatPer100g = 1.2, CarbsPer100g = 0 },
                new ProductItem { Name = "Креветки", CaloriesPer100g = 95, ProteinPer100g = 18.9, FatPer100g = 2.2, CarbsPer100g = 0 },
                
                // Яйця
                new ProductItem { Name = "Яйце куряче (сире)", CaloriesPer100g = 157, ProteinPer100g = 12.7, FatPer100g = 11.5, CarbsPer100g = 0.7 },
                new ProductItem { Name = "Яйце куряче (варене)", CaloriesPer100g = 155, ProteinPer100g = 12.6, FatPer100g = 10.6, CarbsPer100g = 1.1 },
                new ProductItem { Name = "Яєчня (смажена на олії)", CaloriesPer100g = 241, ProteinPer100g = 14, FatPer100g = 20, CarbsPer100g = 1.5 },
                
                // Молочні продукти
                new ProductItem { Name = "Молоко 2.5%", CaloriesPer100g = 52, ProteinPer100g = 2.8, FatPer100g = 2.5, CarbsPer100g = 4.7 },
                new ProductItem { Name = "Молоко 3.2%", CaloriesPer100g = 59, ProteinPer100g = 2.8, FatPer100g = 3.2, CarbsPer100g = 4.7 },
                new ProductItem { Name = "Кефір 1%", CaloriesPer100g = 40, ProteinPer100g = 2.8, FatPer100g = 1, CarbsPer100g = 4 },
                new ProductItem { Name = "Кефір 2.5%", CaloriesPer100g = 50, ProteinPer100g = 2.8, FatPer100g = 2.5, CarbsPer100g = 3.9 },
                new ProductItem { Name = "Сир кисломолочний (творог) 0%", CaloriesPer100g = 71, ProteinPer100g = 16.5, FatPer100g = 0.2, CarbsPer100g = 1.3 },
                new ProductItem { Name = "Сир кисломолочний (творог) 5%", CaloriesPer100g = 121, ProteinPer100g = 17.2, FatPer100g = 5, CarbsPer100g = 1.8 },
                new ProductItem { Name = "Сир кисломолочний (творог) 9%", CaloriesPer100g = 159, ProteinPer100g = 16.7, FatPer100g = 9, CarbsPer100g = 2 },
                new ProductItem { Name = "Сметана 15%", CaloriesPer100g = 158, ProteinPer100g = 2.6, FatPer100g = 15, CarbsPer100g = 3 },
                new ProductItem { Name = "Сметана 20%", CaloriesPer100g = 206, ProteinPer100g = 2.5, FatPer100g = 20, CarbsPer100g = 3.4 },
                new ProductItem { Name = "Сир твердий (Голландський)", CaloriesPer100g = 352, ProteinPer100g = 26, FatPer100g = 26.8, CarbsPer100g = 0 },
                new ProductItem { Name = "Сир Моцарелла", CaloriesPer100g = 280, ProteinPer100g = 28, FatPer100g = 17, CarbsPer100g = 3.1 },
                new ProductItem { Name = "Сир Сулугуні", CaloriesPer100g = 286, ProteinPer100g = 19.5, FatPer100g = 22, CarbsPer100g = 0 },
                new ProductItem { Name = "Вершкове масло 73%", CaloriesPer100g = 661, ProteinPer100g = 0.8, FatPer100g = 73, CarbsPer100g = 1.3 },
                new ProductItem { Name = "Вершкове масло 82%", CaloriesPer100g = 748, ProteinPer100g = 0.5, FatPer100g = 82.5, CarbsPer100g = 0.8 },
                
                // Крупи та макарони (у сухому вигляді)
                new ProductItem { Name = "Гречка (суха)", CaloriesPer100g = 330, ProteinPer100g = 12.6, FatPer100g = 3.3, CarbsPer100g = 64 },
                new ProductItem { Name = "Рис білий (сухий)", CaloriesPer100g = 344, ProteinPer100g = 6.7, FatPer100g = 0.7, CarbsPer100g = 78.9 },
                new ProductItem { Name = "Рис бурий (сухий)", CaloriesPer100g = 337, ProteinPer100g = 7.4, FatPer100g = 1.8, CarbsPer100g = 72.9 },
                new ProductItem { Name = "Вівсянка (суха)", CaloriesPer100g = 352, ProteinPer100g = 12.3, FatPer100g = 6.1, CarbsPer100g = 59.5 },
                new ProductItem { Name = "Макарони з твердих сортів (сухі)", CaloriesPer100g = 344, ProteinPer100g = 10.4, FatPer100g = 1.1, CarbsPer100g = 71.5 },
                new ProductItem { Name = "Булгур (сухий)", CaloriesPer100g = 342, ProteinPer100g = 12.3, FatPer100g = 1.3, CarbsPer100g = 76 },
                new ProductItem { Name = "Кускус (сухий)", CaloriesPer100g = 376, ProteinPer100g = 12.8, FatPer100g = 0.6, CarbsPer100g = 77.4 },
                new ProductItem { Name = "Кіноа (суха)", CaloriesPer100g = 368, ProteinPer100g = 14.1, FatPer100g = 6.1, CarbsPer100g = 64.2 },
                new ProductItem { Name = "Перловка (суха)", CaloriesPer100g = 320, ProteinPer100g = 9.3, FatPer100g = 1.1, CarbsPer100g = 73.7 },
                
                // Крупи та макарони (варені)
                new ProductItem { Name = "Гречка варена", CaloriesPer100g = 110, ProteinPer100g = 4.2, FatPer100g = 1.1, CarbsPer100g = 21.3 },
                new ProductItem { Name = "Рис білий варений", CaloriesPer100g = 116, ProteinPer100g = 2.2, FatPer100g = 0.2, CarbsPer100g = 24.9 },
                new ProductItem { Name = "Макарони варені", CaloriesPer100g = 112, ProteinPer100g = 3.5, FatPer100g = 0.4, CarbsPer100g = 23.2 },
                new ProductItem { Name = "Вівсяна каша (на воді)", CaloriesPer100g = 88, ProteinPer100g = 3, FatPer100g = 1.7, CarbsPer100g = 15 },
                new ProductItem { Name = "Вівсяна каша (на молоці)", CaloriesPer100g = 102, ProteinPer100g = 3.2, FatPer100g = 4.1, CarbsPer100g = 14.2 },

                // Бобові
                new ProductItem { Name = "Квасоля (суха)", CaloriesPer100g = 298, ProteinPer100g = 21, FatPer100g = 2, CarbsPer100g = 47 },
                new ProductItem { Name = "Квасоля варена", CaloriesPer100g = 123, ProteinPer100g = 8.6, FatPer100g = 0.5, CarbsPer100g = 22.8 },
                new ProductItem { Name = "Сочевиця (суха)", CaloriesPer100g = 295, ProteinPer100g = 24, FatPer100g = 1.5, CarbsPer100g = 46.3 },
                new ProductItem { Name = "Сочевиця варена", CaloriesPer100g = 112, ProteinPer100g = 7.8, FatPer100g = 0.4, CarbsPer100g = 20 },
                new ProductItem { Name = "Нут (сухий)", CaloriesPer100g = 364, ProteinPer100g = 19, FatPer100g = 6, CarbsPer100g = 61 },
                new ProductItem { Name = "Горох (сухий)", CaloriesPer100g = 298, ProteinPer100g = 20.5, FatPer100g = 2, CarbsPer100g = 49.5 },
                
                // Овочі та зелень
                new ProductItem { Name = "Картопля сира", CaloriesPer100g = 77, ProteinPer100g = 2, FatPer100g = 0.4, CarbsPer100g = 16.3 },
                new ProductItem { Name = "Картопля варена", CaloriesPer100g = 82, ProteinPer100g = 2, FatPer100g = 0.4, CarbsPer100g = 16.7 },
                new ProductItem { Name = "Картопля смажена", CaloriesPer100g = 192, ProteinPer100g = 2.8, FatPer100g = 9.5, CarbsPer100g = 23.4 },
                new ProductItem { Name = "Картопляне пюре (з молоком і маслом)", CaloriesPer100g = 106, ProteinPer100g = 2.2, FatPer100g = 4.2, CarbsPer100g = 14.7 },
                new ProductItem { Name = "Помідор", CaloriesPer100g = 20, ProteinPer100g = 1.1, FatPer100g = 0.2, CarbsPer100g = 3.8 },
                new ProductItem { Name = "Огірок", CaloriesPer100g = 15, ProteinPer100g = 0.8, FatPer100g = 0.1, CarbsPer100g = 2.8 },
                new ProductItem { Name = "Перець солодкий", CaloriesPer100g = 27, ProteinPer100g = 1.3, FatPer100g = 0, CarbsPer100g = 5.3 },
                new ProductItem { Name = "Капуста білокачанна", CaloriesPer100g = 27, ProteinPer100g = 1.8, FatPer100g = 0.1, CarbsPer100g = 4.7 },
                new ProductItem { Name = "Морква", CaloriesPer100g = 35, ProteinPer100g = 1.3, FatPer100g = 0.1, CarbsPer100g = 6.9 },
                new ProductItem { Name = "Буряк", CaloriesPer100g = 43, ProteinPer100g = 1.5, FatPer100g = 0.1, CarbsPer100g = 8.8 },
                new ProductItem { Name = "Цибуля ріпчаста", CaloriesPer100g = 41, ProteinPer100g = 1.4, FatPer100g = 0.2, CarbsPer100g = 8.2 },
                new ProductItem { Name = "Часник", CaloriesPer100g = 149, ProteinPer100g = 6.5, FatPer100g = 0.5, CarbsPer100g = 29.9 },
                new ProductItem { Name = "Кабачок", CaloriesPer100g = 24, ProteinPer100g = 0.6, FatPer100g = 0.3, CarbsPer100g = 4.6 },
                new ProductItem { Name = "Баклажан", CaloriesPer100g = 24, ProteinPer100g = 1.2, FatPer100g = 0.1, CarbsPer100g = 4.5 },
                new ProductItem { Name = "Броколі", CaloriesPer100g = 34, ProteinPer100g = 2.8, FatPer100g = 0.4, CarbsPer100g = 6.6 },
                new ProductItem { Name = "Шпинат", CaloriesPer100g = 22, ProteinPer100g = 2.9, FatPer100g = 0.3, CarbsPer100g = 2 },
                new ProductItem { Name = "Авокадо", CaloriesPer100g = 160, ProteinPer100g = 2, FatPer100g = 14.7, CarbsPer100g = 8.5 },
                new ProductItem { Name = "Печериці (сирі)", CaloriesPer100g = 27, ProteinPer100g = 4.3, FatPer100g = 1, CarbsPer100g = 0.1 },
                
                // Фрукти та ягоди
                new ProductItem { Name = "Яблуко", CaloriesPer100g = 47, ProteinPer100g = 0.4, FatPer100g = 0.4, CarbsPer100g = 9.8 },
                new ProductItem { Name = "Банан", CaloriesPer100g = 89, ProteinPer100g = 1.5, FatPer100g = 0.1, CarbsPer100g = 21.8 },
                new ProductItem { Name = "Апельсин", CaloriesPer100g = 43, ProteinPer100g = 0.9, FatPer100g = 0.2, CarbsPer100g = 8.1 },
                new ProductItem { Name = "Мандарин", CaloriesPer100g = 38, ProteinPer100g = 0.8, FatPer100g = 0.2, CarbsPer100g = 7.5 },
                new ProductItem { Name = "Лимон", CaloriesPer100g = 34, ProteinPer100g = 0.9, FatPer100g = 0.1, CarbsPer100g = 3 },
                new ProductItem { Name = "Грейпфрут", CaloriesPer100g = 35, ProteinPer100g = 0.7, FatPer100g = 0.2, CarbsPer100g = 6.5 },
                new ProductItem { Name = "Груша", CaloriesPer100g = 47, ProteinPer100g = 0.4, FatPer100g = 0.3, CarbsPer100g = 10.3 },
                new ProductItem { Name = "Ківі", CaloriesPer100g = 47, ProteinPer100g = 0.8, FatPer100g = 0.4, CarbsPer100g = 8.1 },
                new ProductItem { Name = "Виноград", CaloriesPer100g = 69, ProteinPer100g = 0.6, FatPer100g = 0.2, CarbsPer100g = 15.4 },
                new ProductItem { Name = "Полуниця", CaloriesPer100g = 32, ProteinPer100g = 0.8, FatPer100g = 0.4, CarbsPer100g = 7.5 },
                new ProductItem { Name = "Малина", CaloriesPer100g = 42, ProteinPer100g = 0.8, FatPer100g = 0.5, CarbsPer100g = 8.3 },
                new ProductItem { Name = "Черешня", CaloriesPer100g = 52, ProteinPer100g = 1.1, FatPer100g = 0.4, CarbsPer100g = 10.6 },
                new ProductItem { Name = "Кавун", CaloriesPer100g = 27, ProteinPer100g = 0.6, FatPer100g = 0.1, CarbsPer100g = 5.8 },
                new ProductItem { Name = "Диня", CaloriesPer100g = 33, ProteinPer100g = 0.6, FatPer100g = 0.3, CarbsPer100g = 7.4 },
                
                // Хліб та випічка
                new ProductItem { Name = "Хліб білий (пшеничний)", CaloriesPer100g = 265, ProteinPer100g = 7.5, FatPer100g = 2.9, CarbsPer100g = 51.4 },
                new ProductItem { Name = "Хліб чорний (житній)", CaloriesPer100g = 214, ProteinPer100g = 5.6, FatPer100g = 1.1, CarbsPer100g = 43.1 },
                new ProductItem { Name = "Хліб цільнозерновий", CaloriesPer100g = 228, ProteinPer100g = 9.9, FatPer100g = 2.2, CarbsPer100g = 43.4 },
                new ProductItem { Name = "Батон", CaloriesPer100g = 264, ProteinPer100g = 7.5, FatPer100g = 2.9, CarbsPer100g = 50.9 },
                new ProductItem { Name = "Лаваш", CaloriesPer100g = 277, ProteinPer100g = 9.1, FatPer100g = 1.2, CarbsPer100g = 56.1 },
                new ProductItem { Name = "Круасан (без начинки)", CaloriesPer100g = 406, ProteinPer100g = 8.2, FatPer100g = 21, CarbsPer100g = 45.8 },
                
                // Горіхи, насіння, олії
                new ProductItem { Name = "Волоський горіх", CaloriesPer100g = 654, ProteinPer100g = 15.2, FatPer100g = 65.2, CarbsPer100g = 13.7 },
                new ProductItem { Name = "Мигдаль", CaloriesPer100g = 579, ProteinPer100g = 21.1, FatPer100g = 49.9, CarbsPer100g = 21.6 },
                new ProductItem { Name = "Фундук", CaloriesPer100g = 628, ProteinPer100g = 15, FatPer100g = 61, CarbsPer100g = 17 },
                new ProductItem { Name = "Арахіс (смажений)", CaloriesPer100g = 585, ProteinPer100g = 23.7, FatPer100g = 49.7, CarbsPer100g = 21.3 },
                new ProductItem { Name = "Насіння соняшника (очищене)", CaloriesPer100g = 584, ProteinPer100g = 20.7, FatPer100g = 51.5, CarbsPer100g = 20 },
                new ProductItem { Name = "Гарбузове насіння", CaloriesPer100g = 559, ProteinPer100g = 30.2, FatPer100g = 49.1, CarbsPer100g = 10.7 },
                new ProductItem { Name = "Олія соняшникова", CaloriesPer100g = 899, ProteinPer100g = 0, FatPer100g = 99.9, CarbsPer100g = 0 },
                new ProductItem { Name = "Олія оливкова", CaloriesPer100g = 898, ProteinPer100g = 0, FatPer100g = 99.8, CarbsPer100g = 0 },
                
                // Солодощі та цукор
                new ProductItem { Name = "Цукор", CaloriesPer100g = 398, ProteinPer100g = 0, FatPer100g = 0, CarbsPer100g = 99.7 },
                new ProductItem { Name = "Мед", CaloriesPer100g = 304, ProteinPer100g = 0.3, FatPer100g = 0, CarbsPer100g = 82.4 },
                new ProductItem { Name = "Шоколад молочний", CaloriesPer100g = 535, ProteinPer100g = 7.6, FatPer100g = 29.7, CarbsPer100g = 59.4 },
                new ProductItem { Name = "Шоколад чорний (70%)", CaloriesPer100g = 539, ProteinPer100g = 6.2, FatPer100g = 35.4, CarbsPer100g = 48.2 },
                new ProductItem { Name = "Печиво вівсяне", CaloriesPer100g = 437, ProteinPer100g = 6.5, FatPer100g = 14.4, CarbsPer100g = 71.8 },
                new ProductItem { Name = "Зефір", CaloriesPer100g = 326, ProteinPer100g = 0.8, FatPer100g = 0.1, CarbsPer100g = 79.8 },
                new ProductItem { Name = "Мармелад", CaloriesPer100g = 321, ProteinPer100g = 0.1, FatPer100g = 0, CarbsPer100g = 79.4 },
                
                // Напої
                new ProductItem { Name = "Сік яблучний", CaloriesPer100g = 46, ProteinPer100g = 0.1, FatPer100g = 0.1, CarbsPer100g = 11.3 },
                new ProductItem { Name = "Сік апельсиновий", CaloriesPer100g = 45, ProteinPer100g = 0.7, FatPer100g = 0.2, CarbsPer100g = 10.4 },
                new ProductItem { Name = "Кола", CaloriesPer100g = 42, ProteinPer100g = 0, FatPer100g = 0, CarbsPer100g = 10.6 },
                new ProductItem { Name = "Кава (без цукру)", CaloriesPer100g = 2, ProteinPer100g = 0.2, FatPer100g = 0.1, CarbsPer100g = 0.1 },
                new ProductItem { Name = "Чай чорний (без цукру)", CaloriesPer100g = 1, ProteinPer100g = 0.1, FatPer100g = 0, CarbsPer100g = 0.3 },
                
                // Страви української кухні
                new ProductItem { Name = "Борщ український (з м'ясом)", CaloriesPer100g = 57, ProteinPer100g = 2.8, FatPer100g = 3.6, CarbsPer100g = 3.7 },
                new ProductItem { Name = "Борщ пісний", CaloriesPer100g = 28, ProteinPer100g = 1.2, FatPer100g = 0.5, CarbsPer100g = 4.8 },
                new ProductItem { Name = "Вареники з картоплею", CaloriesPer100g = 220, ProteinPer100g = 5.2, FatPer100g = 4.5, CarbsPer100g = 39.8 },
                new ProductItem { Name = "Вареники з сиром", CaloriesPer100g = 245, ProteinPer100g = 10.1, FatPer100g = 6.2, CarbsPer100g = 36.4 },
                new ProductItem { Name = "Вареники з вишнею", CaloriesPer100g = 188, ProteinPer100g = 3.8, FatPer100g = 1.1, CarbsPer100g = 38.6 },
                new ProductItem { Name = "Пельмені", CaloriesPer100g = 275, ProteinPer100g = 11.9, FatPer100g = 12.4, CarbsPer100g = 29 },
                new ProductItem { Name = "Голубці з м'ясом", CaloriesPer100g = 105, ProteinPer100g = 4.5, FatPer100g = 5.2, CarbsPer100g = 10.5 },
                new ProductItem { Name = "Млинці (без начинки)", CaloriesPer100g = 232, ProteinPer100g = 6.1, FatPer100g = 10.5, CarbsPer100g = 28.5 },
                new ProductItem { Name = "Сирники (смажені)", CaloriesPer100g = 250, ProteinPer100g = 13.5, FatPer100g = 11, CarbsPer100g = 24 },
                new ProductItem { Name = "Котлета куряча", CaloriesPer100g = 168, ProteinPer100g = 15.5, FatPer100g = 7.5, CarbsPer100g = 8.5 },
                new ProductItem { Name = "Котлета свинна", CaloriesPer100g = 275, ProteinPer100g = 12.8, FatPer100g = 20.5, CarbsPer100g = 9 },
                new ProductItem { Name = "Відбивна куряча", CaloriesPer100g = 185, ProteinPer100g = 18, FatPer100g = 8.5, CarbsPer100g = 9.2 },
                new ProductItem { Name = "Відбивна свинна", CaloriesPer100g = 280, ProteinPer100g = 14, FatPer100g = 21, CarbsPer100g = 8.5 },
                
                // Інше
                new ProductItem { Name = "Майонез 67%", CaloriesPer100g = 627, ProteinPer100g = 0.5, FatPer100g = 67, CarbsPer100g = 2.4 },
                new ProductItem { Name = "Кетчуп", CaloriesPer100g = 93, ProteinPer100g = 1.8, FatPer100g = 0, CarbsPer100g = 21.4 },
                new ProductItem { Name = "Гірчиця", CaloriesPer100g = 162, ProteinPer100g = 5.4, FatPer100g = 5.3, CarbsPer100g = 23 },
                new ProductItem { Name = "Піца (Маргарита)", CaloriesPer100g = 266, ProteinPer100g = 11, FatPer100g = 10, CarbsPer100g = 33 },
                new ProductItem { Name = "Шаурма", CaloriesPer100g = 220, ProteinPer100g = 9, FatPer100g = 12, CarbsPer100g = 20 }
            };

            await context.ProductItems.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
