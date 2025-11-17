using Microsoft.EntityFrameworkCore;
using FitnessEngine.Api.Models;

namespace FitnessEngine.Api.Repositories
{
    public class UserLogsRepository
    {
        private readonly FitnessDbContext _context;

        public UserLogsRepository(FitnessDbContext context) => _context = context;

        public async Task AddUserLogAsync(UserLog log)
        {
            _context.UserLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUserPointsAsync(int userId)
        {
            var logs = await _context.UserLogs.Include(l => l.Workout)
                .Where(l => l.UserId == userId).ToListAsync();
            return logs.Sum(l => l.DurationMinutes * (l.Workout?.Difficulty ?? 1));
        }

        public async Task<List<string>> GetUserBadges(int userId)
        {
            int points = await GetUserPointsAsync(userId);
            var badges = new List<string>();
            if (points >= 50) badges.Add("Bronze");
            if (points >= 100) badges.Add("Silver");
            if (points >= 200) badges.Add("Gold");
            return badges;
        }

        public async Task<List<UserLog>> GetUserWorkoutHistory(int userId, int lastDays = 7)
        {
            var since = DateTime.UtcNow.AddDays(-lastDays);
            return await _context.UserLogs
                .Where(l => l.UserId == userId && l.CompletedAt >= since)
                .Include(l => l.Workout)
                .ToListAsync();
        }
    }
}
