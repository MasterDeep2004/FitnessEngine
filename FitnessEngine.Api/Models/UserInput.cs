namespace FitnessEngine.Api.Models
{
    public class UserInput
    {
        public int Age { get; set; }
        public float Weight { get; set; }
        public string Goal { get; set; }
        public string FitnessLevel { get; set; }
        public string PreferredType { get; set; }
        public int? MaxDuration { get; set; }
    }
}
