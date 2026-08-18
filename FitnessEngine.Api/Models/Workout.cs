namespace FitnessEngine.Api.Models;

public class Workout
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FitnessLevel { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public string Goal { get; set; } = string.Empty;

    public string? Type { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
