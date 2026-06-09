using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.Services;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<WeatherController> _logger;
        private readonly WeatherService _weatherService;

        public WeatherController(AppDbContext context, ILogger<WeatherController> logger, WeatherService weatherService)
        {
            _context = context;
            _logger = logger;
            _weatherService = weatherService;
        }

        // ✅ CURRENT WEATHER
        [HttpGet("{destinationId}")]
        public async Task<IActionResult> GetCurrent(int destinationId)
        {
            _logger.LogInformation(
                "Fetching current weather for DestinationId: {DestinationId}",
                destinationId);

            var weather = await _context.Weather
                .Where(w => w.DestinationId == destinationId)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();

            if (weather == null)
            {
                _logger.LogWarning(
                    "Current weather not found for DestinationId: {DestinationId}",
                    destinationId);

                return NotFound();
            }

            _logger.LogInformation(
               "Current weather retrieved successfully for DestinationId: {DestinationId}",
               destinationId);

            return Ok(weather);
        }

        // ✅ FORECAST
        [HttpGet("forecast/{destinationId}")]
        public async Task<IActionResult> GetForecast(int destinationId)
        {
            _logger.LogInformation(
               "Fetching weather forecast for DestinationId: {DestinationId}",
               destinationId);

            var forecast = await _context.Weather
                .Where(w => w.DestinationId == destinationId)
                .OrderBy(w => w.Date)
                .ToListAsync();

            if (forecast.Count == 0)
            {
                _logger.LogWarning(
                    "No weather forecast found for DestinationId: {DestinationId}",
                    destinationId);
            }

            _logger.LogInformation(
                "Retrieved {ForecastCount} forecast records for DestinationId: {DestinationId}",
                forecast.Count,
                destinationId);


            return Ok(forecast);
        }
    }
}
