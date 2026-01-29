using BjuApiServer.Models;
using System.Text.Json;

namespace BjuApiServer.Services
{
    public class JsonDbService
    {
        private readonly string _filePath = "users.json";
        private List<User> _users = new();

        public JsonDbService()
        {
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
            }
        }

        private void SaveUsers()
        {
            var json = JsonSerializer.Serialize(_users);
            File.WriteAllText(_filePath, json);
        }

        public User? GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public User? GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public int GetNextUserId()
        {
            return _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
        }

        public void AddUser(User user)
        {
            _users.Add(user);
            SaveUsers();
        }

        public void UpdateUser(User user)
        {
            var existing = _users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                _users.Remove(existing);
                _users.Add(user);
                SaveUsers();
            }
        }

        public List<User> GetAllUsers()
        {
            return _users;
        }
    }
}