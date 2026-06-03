using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WeatherController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ CURRENT WEATHER
        [HttpGet("{destinationId}")]
        public async Task<IActionResult> GetCurrent(int destinationId)
        {
            var weather = await _context.Weather
                .Where(w => w.DestinationId == destinationId)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();

            if (weather == null)
                return NotFound();

            return Ok(weather);
        }

        // ✅ FORECAST
        [HttpGet("forecast/{destinationId}")]
        public async Task<IActionResult> GetForecast(int destinationId)
        {
            var forecast = await _context.Weather
                .Where(w => w.DestinationId == destinationId)
                .OrderBy(w => w.Date)
                .ToListAsync();

            return Ok(forecast);
        }
    }
}
