namespace FitnessEngine.Api.Models
{
    public class DietPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Goal { get; set; }
        public int Calories { get; set; }
        public int Protein { get; set; }
        public int Carbs { get; set; }
        public int Fats { get; set; }
        public string Description { get; set; }
    }
}
