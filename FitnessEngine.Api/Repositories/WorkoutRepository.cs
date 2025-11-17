using Microsoft.EntityFrameworkCore;
using FitnessEngine.Api.Models;

namespace FitnessEngine.Api.Repositories
{
    public class WorkoutRepository
    {
        private readonly FitnessDbContext _context;

        public WorkoutRepository(FitnessDbContext context) => _context = context;

        public async Task<List<Workout>> GetWorkoutsAsync(string goal = null, string fitnessLevel = null)
        {
            var query = _context.Workouts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(goal)) query = query.Where(w => w.Goal == goal);
            if (!string.IsNullOrWhiteSpace(fitnessLevel)) query = query.Where(w => w.FitnessLevel == fitnessLevel);
            return await query.ToListAsync();
        }

        public async Task<Workout?> GetWorkoutByIdAsync(int id)
            => await _context.Workouts.FirstOrDefaultAsync(w => w.Id == id);
    }
}
