using FitnessEngine.Api.DTOs;
using FitnessEngine.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessEngine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly RecommendationService _recommendationService;

    public RecommendationController(
        RecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpPost]
    public async Task<ActionResult<RecommendationDto>>
        GenerateRecommendation(
            [FromBody] UserInputDto input)
    {
        if (input.Age <= 0)
            return BadRequest("Age must be greater than zero.");

        if (input.Weight <= 0)
            return BadRequest("Weight must be greater than zero.");

        if (string.IsNullOrWhiteSpace(input.Goal))
            return BadRequest("Goal is required.");

        if (string.IsNullOrWhiteSpace(input.FitnessLevel))
            return BadRequest("Fitness level is required.");

        try
        {
            var recommendation =
                await _recommendationService
                    .GenerateRecommendationAsync(input);

            return Ok(recommendation);
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new
                {
                    message = "Failed to generate recommendation.",
                    error = ex.Message
                }
            );
        }
    }
}
