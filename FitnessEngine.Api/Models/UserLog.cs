namespace FitnessEngine.Api.Models
{
    public class UserLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
        public int DurationMinutes { get; set; }
        public int CaloriesBurned { get; set; }
        public int PointsEarned { get; set; }

        public Workout Workout { get; set; }
    }
}
