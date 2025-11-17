using FitnessEngine.Api.Models;
using FitnessEngine.Api.Repositories;
using System.Text.Json;

namespace FitnessEngine.Api.Services
{
    public class RecommendationService
    {
        private readonly WorkoutRepository _workoutRepo;
        private readonly HttpClient _http;

        public RecommendationService(WorkoutRepository workoutRepo, IHttpClientFactory httpFactory)
        {
            _workoutRepo = workoutRepo;
            _http = httpFactory.CreateClient();
        }

        // Updated: Removed userId since we don't need it
        public async Task<(List<Workout> Workouts, DietPlan DietPlan)> GetPersonalizedRecommendationsAsync(UserInput input)
        {
            // Fetch workouts from DB as fallback
            var workouts = await _workoutRepo.GetWorkoutsAsync(input.Goal, input.FitnessLevel);

            // Prepare prompt for LLM
            string prompt = $"User info: Age {input.Age}, Weight {input.Weight}, Goal {input.Goal}, Level {input.FitnessLevel}, PreferredType {input.PreferredType}, MaxDuration {input.MaxDuration}. Return top 5 workouts and a diet plan as JSON.";

            // Call LLM
            var llmResponse = await QueryLLMAsync(prompt);

            var recommendation = JsonSerializer.Deserialize<LLMRecommendation>(llmResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Use LLM results or fallback
            var finalWorkouts = recommendation?.Workouts ?? workouts.Take(5).ToList();
            var dietPlan = recommendation?.DietPlan ?? GenerateDietPlan(input);

            return (finalWorkouts, dietPlan);
        }

        private DietPlan GenerateDietPlan(UserInput input)
        {
            int calories = input.Goal.ToLowerInvariant() switch
            {
                "weight loss" => 1800,
                "muscle gain" => 2500,
                "endurance" => 2200,
                _ => 2000
            };

            return new DietPlan
            {
                Calories = calories,
                Protein = input.Goal.Equals("muscle gain", StringComparison.OrdinalIgnoreCase) ? 150 : 120,
                Carbs = input.Goal.Equals("weight loss", StringComparison.OrdinalIgnoreCase) ? 150 : 250,
                Fats = 60,
                Description = $"Dynamic diet plan for {input.Goal}"
            };
        }

        private async Task<string> QueryLLMAsync(string prompt)
        {
            await Task.Delay(100); // simulate network call
            return @"{
        ""Workouts"": [
            { ""Name"": ""Push-ups"", ""Description"": ""Do push-ups"", ""FitnessLevel"": ""Beginner"", ""DurationMinutes"": 15, ""Goal"": ""Muscle Gain"" },
            { ""Name"": ""Jogging"", ""Description"": ""30 mins jogging"", ""FitnessLevel"": ""Beginner"", ""DurationMinutes"": 30, ""Goal"": ""Endurance"" }
        ],
        ""DietPlan"": {
            ""Calories"": 2000,
            ""Protein"": 150,
            ""Carbs"": 200,
            ""Fats"": 60,
            ""Description"": ""Sample diet plan""
        }
    }";
        }

    }

    public class LLMRecommendation
    {
        public List<Workout> Workouts { get; set; }
        public DietPlan DietPlan { get; set; }
    }
}
