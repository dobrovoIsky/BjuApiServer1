/*using System.Text.Json;
using BjuApiServer.DTO;
using BjuApiServer.Models;

namespace BjuApiServer.Services;

public class JsonDbService
{
    private readonly string _filePath;
    private readonly ILogger<JsonDbService> _logger;

    public JsonDbService(IWebHostEnvironment env, ILogger<JsonDbService> logger)
    {
        _filePath = Path.Combine(env.ContentRootPath, "wwwroot", "users.json");
        _logger = logger;
    }

    private async Task<List<User>> ReadUsersAsync()
    {
        if (!File.Exists(_filePath))
        {
            return new List<User>();
        }
        try
        {
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not read or deserialize users.json");
            return new List<User>();
        }
    }

    private async Task WriteUsersAsync(List<User> users)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(users, options);
        var directory = Path.GetDirectoryName(_filePath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        await File.WriteAllTextAsync(_filePath, json);
    }

    public async Task<(bool Success, string Error, User? User)> RegisterUserAsync(UserRegDTO newUserDto)
    {
        var users = await ReadUsersAsync();
        if (users.Any(u => string.Equals(u.Username, newUserDto.Username, StringComparison.OrdinalIgnoreCase)))
        {
            return (false, "Username already exists.", null);
        }

        var user = new User
        {
            Username = newUserDto.Username,
            PasswordHash = newUserDto.PasswordHash,
            Height = newUserDto.Height,
            Weight = newUserDto.Weight,
            Age = newUserDto.Age,
            Goal = newUserDto.Goal,
            ActivityLevel = newUserDto.ActivityLevel,
            // Встановлюємо значення за замовчуванням при реєстрації
            Theme = "light",
            Language = "uk"
        };

        user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        await WriteUsersAsync(users);
        return (true, string.Empty, user);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        var users = await ReadUsersAsync();
        return users.FirstOrDefault(u => u.Id == id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var users = await ReadUsersAsync();
        return users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<User?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var users = await ReadUsersAsync();
        var userToUpdate = users.FirstOrDefault(u => u.Id == id);

        if (userToUpdate == null)
        {
            return null; // Користувача не знайдено
        }

        // Оновлюємо поля
        userToUpdate.Height = updateUserDto.Height;
        userToUpdate.Weight = updateUserDto.Weight;
        userToUpdate.Age = updateUserDto.Age;
        userToUpdate.Goal = updateUserDto.Goal;
        userToUpdate.ActivityLevel = updateUserDto.ActivityLevel;

        await WriteUsersAsync(users);
        return userToUpdate;
    }

    // Новий метод для оновлення налаштувань
    public async Task<User?> UpdateUserSettingsAsync(int id, SettingsDto settingsDto)
    {
        var users = await ReadUsersAsync();
        var userToUpdate = users.FirstOrDefault(u => u.Id == id);

        if (userToUpdate == null)
        {
            return null;
        }

        userToUpdate.Theme = settingsDto.Theme ?? userToUpdate.Theme;
        userToUpdate.Language = settingsDto.Language ?? userToUpdate.Language;

        await WriteUsersAsync(users);
        return userToUpdate;
    }
}*/