@echo off
echo ====================================================
echo Оновлення бази даних (Додавання MealType)
echo ====================================================
echo Зачекайте, виконується міграція...

dotnet ef migrations add AddMealTypeToFoodEntry
if %errorlevel% neq 0 (
    echo Помилка створення міграції! Можливо, dotnet-ef не встановлено.
    echo Спроба встановити dotnet-ef...
    dotnet tool install --global dotnet-ef
    dotnet ef migrations add AddMealTypeToFoodEntry
)

dotnet ef database update
if %errorlevel% neq 0 (
    echo Помилка оновлення бази даних! Переконайтеся, що сервер зараз вимкнено.
) else (
    echo.
    echo ====================================================
    echo Успішно! База даних оновлена.
    echo Тепер ви можете запустити сервер знову.
    echo ====================================================
)
pause
