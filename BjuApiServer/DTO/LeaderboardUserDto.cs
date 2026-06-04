using System;

namespace BjuApiServer.DTO
{
    public class LeaderboardUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int AvatarId { get; set; }
        public int Points { get; set; }
        public int CurrentStreak { get; set; }
    }
}
