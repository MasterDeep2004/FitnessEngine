using FitnessEngine.Api.Data;
using FitnessEngine.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessEngine.Api.Repositories;

public class WorkoutRepository
{
    private readonly FitnessDbContext _context;

    public WorkoutRepository(FitnessDbContext context)
    {
        _context = context;
    }

    public async Task<List<Workout>> GetAllAsync()
    {
        return await _context.Workouts
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Workout?> GetByIdAsync(int id)
    {
        return await _context.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Workout> CreateAsync(Workout workout)
    {
        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        return workout;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var workout = await _context.Workouts.FindAsync(id);

        if (workout == null)
            return false;

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();

        return true;
    }
}
