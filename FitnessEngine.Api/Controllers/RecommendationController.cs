using FitnessEngine.Api.Models;
using FitnessEngine.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _service;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(RecommendationService service, ILogger<RecommendationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("llm")]
        public async Task<IActionResult> GetLLMRecommendations([FromBody] UserInput input)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid input");

            try
            {
                // Call service with only UserInput
                var result = await _service.GetPersonalizedRecommendationsAsync(input);

                return Ok(new
                {
                    Workouts = result.Workouts,
                    DietPlan = result.DietPlan
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching LLM recommendations");
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
