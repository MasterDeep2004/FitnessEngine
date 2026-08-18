namespace FitnessEngine.Api.DTOs;

public class RecommendationDto
{
    public List<WorkoutRecommendationDto> Workouts { get; set; } = new();

    public DietPlanDto DietPlan { get; set; } = new();
}

public class WorkoutRecommendationDto
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FitnessLevel { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public string Goal { get; set; } = string.Empty;
}

public class DietPlanDto
{
    public int Calories { get; set; }

    public int Protein { get; set; }

    public int Carbs { get; set; }

    public int Fats { get; set; }

    public string Description { get; set; } = string.Empty;
}
