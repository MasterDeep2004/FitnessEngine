namespace FitnessEngine.Api.DTOs;

public class UserInputDto
{
    public int Age { get; set; }

    public double Weight { get; set; }

    public string Goal { get; set; } = string.Empty;

    public string FitnessLevel { get; set; } = string.Empty;

    public string? PreferredType { get; set; }

    public int? MaxDuration { get; set; }
}
