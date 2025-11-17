using FitnessEngine.Api.Models;
using FitnessEngine.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FitnessEngine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly FitnessDbContext _context;

        public UsersController(FitnessDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("stream")]
        public async Task StreamUsers()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");

            string? lastJson = null;

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                var users = await _context.Users
                    .GroupBy(u => u.Id)
                    .Select(g => g.First())
                    .ToListAsync();

                var json = JsonSerializer.Serialize(users);

                if (json != lastJson)
                {
                    await Response.WriteAsync($"data: {json}\n\n");
                    await Response.Body.FlushAsync();
                    lastJson = json;
                }

                await Task.Delay(5000);
            }
        }
    }
}
