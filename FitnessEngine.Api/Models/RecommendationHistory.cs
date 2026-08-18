namespace FitnessEngine.Api.Models;

public class RecommendationHistory
{
    public int Id { get; set; }

    public int UserProfileId { get; set; }

    public string RecommendationJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public UserProfile? UserProfile { get; set; }
}
