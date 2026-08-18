using System.Text;
using System.Text.Json;
using FitnessEngine.Api.Data;
using FitnessEngine.Api.DTOs;
using FitnessEngine.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessEngine.Api.Services;

public class RecommendationService
{
    private readonly HttpClient _httpClient;
    private readonly FitnessDbContext _context;
    private readonly ElasticsearchService _elasticsearch;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RecommendationService> _logger;

    public RecommendationService(
        HttpClient httpClient,
        FitnessDbContext context,
        ElasticsearchService elasticsearch,
        IConfiguration configuration,
        ILogger<RecommendationService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _elasticsearch = elasticsearch;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<RecommendationDto> GenerateRecommendationAsync(
        UserInputDto input)
    {
        _logger.LogInformation(
            "Generating fitness recommendation for Goal={Goal}, FitnessLevel={FitnessLevel}",
            input.Goal,
            input.FitnessLevel
        );

        var user = new UserProfile
        {
            Age = input.Age,
            Weight = input.Weight,
            Goal = input.Goal,
            FitnessLevel = input.FitnessLevel,
            PreferredType = input.PreferredType,
            MaxDuration = input.MaxDuration
        };

        _context.UserProfiles.Add(user);

        await _context.SaveChangesAsync();

        var matchingWorkouts =
            await _elasticsearch.SearchWorkoutsAsync(
                input.PreferredType,
                input.Goal,
                input.FitnessLevel
            );

        var workoutContext = string.Join(
            "\n",
            matchingWorkouts.Take(10).Select(w =>
                $"- {w.Name}: {w.Description}, {w.DurationMinutes} min"
            )
        );

        var prompt = $"""
        You are a professional fitness trainer and nutritionist.

        User:
        Age: {input.Age}
        Weight: {input.Weight}
        Goal: {input.Goal}
        Fitness Level: {input.FitnessLevel}
        Preferred Type: {input.PreferredType ?? "Any"}
        Maximum Duration: {input.MaxDuration?.ToString() ?? "Any"}

        Available workouts from the database:
        {workoutContext}

        Generate a personalized workout and diet plan.

        Return ONLY valid JSON in this format:

        {{
          "Workouts": [
            {{
              "Name": "string",
              "Description": "string",
              "FitnessLevel": "string",
              "DurationMinutes": 30,
              "Goal": "string"
            }}
          ],
          "DietPlan": {{
            "Calories": 2000,
            "Protein": 150,
            "Carbs": 200,
            "Fats": 60,
            "Description": "string"
          }}
        }}
        """;

        var ollamaUrl =
            _configuration["Ollama:Uri"]
            ?? "http://127.0.0.1:11434/api/chat";

        var model =
            _configuration["Ollama:Model"]
            ?? "phi3:mini";

        var requestBody = new
        {
            model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are a helpful AI fitness assistant."
                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            stream = false
        };

        using var requestContent = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        using var response = await _httpClient.PostAsync(
            ollamaUrl,
            requestContent
        );

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Ollama request failed with status code {StatusCode}",
                response.StatusCode
            );

            throw new Exception("AI recommendation service failed.");
        }

        var responseJson =
            await response.Content.ReadAsStringAsync();

        using var document =
            JsonDocument.Parse(responseJson);

        var content =
            document.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new Exception("AI returned an empty response.");
        }

        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');

        if (start < 0 || end < start)
        {
            throw new Exception("AI returned invalid JSON.");
        }

        var json = content[start..(end + 1)];

        var recommendation =
            JsonSerializer.Deserialize<RecommendationDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );

        if (recommendation == null)
        {
            throw new Exception(
                "Could not deserialize AI recommendation."
            );
        }

        var history = new RecommendationHistory
        {
            UserProfileId = user.Id,
            RecommendationJson = json
        };

        _context.RecommendationHistories.Add(history);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Fitness recommendation generated successfully for UserId={UserId}",
            user.Id
        );

        return recommendation;
    }
}
