using FitnessEngine.Api.Models;
using FitnessEngine.Api.Repositories;
using FitnessEngine.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessEngine.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkoutController : ControllerBase
{
    private readonly WorkoutRepository _repository;
    private readonly ElasticsearchService _elasticsearch;

    public WorkoutController(
        WorkoutRepository repository,
        ElasticsearchService elasticsearch)
    {
        _repository = repository;
        _elasticsearch = elasticsearch;
    }

    [HttpGet]
    public async Task<ActionResult<List<Workout>>> GetAll()
    {
        var workouts = await _repository.GetAllAsync();

        return Ok(workouts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Workout>> GetById(int id)
    {
        var workout = await _repository.GetByIdAsync(id);

        if (workout == null)
            return NotFound();

        return Ok(workout);
    }

    [HttpPost]
    public async Task<ActionResult<Workout>> Create(
        [FromBody] Workout workout)
    {
        if (string.IsNullOrWhiteSpace(workout.Name))
            return BadRequest("Workout name is required.");

        var created =
            await _repository.CreateAsync(workout);

        await _elasticsearch.IndexWorkoutAsync(created);

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted =
            await _repository.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Workout>>> Search(
        [FromQuery] string? query,
        [FromQuery] string? goal,
        [FromQuery] string? fitnessLevel)
    {
        var results =
            await _elasticsearch.SearchWorkoutsAsync(
                query,
                goal,
                fitnessLevel
            );

        return Ok(results);
    }
}
