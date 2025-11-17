using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessEngine.Api.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Goal { get; set; }

        [Column("Level")]
        public string FitnessLevel { get; set; }

        public int DurationMinutes { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public int Difficulty => FitnessLevel switch
        {
            "Beginner" => 3,
            "Intermediate" => 6,
            "Advanced" => 9,
            _ => 3
        };
    }
}
