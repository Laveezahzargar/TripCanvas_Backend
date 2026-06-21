using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.DTOs;
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

            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == destinationId);

            if (destination == null)
                return NotFound("Destination not found.");

            var weather = await _weatherService.GetWeather(
                destination.Latitude,
                destination.Longitude);

            if (weather == null)
            {
                _logger.LogError(
                    "Failed to retrieve weather for DestinationId: {DestinationId}",
                    destinationId);

                return StatusCode(500, "Failed to retrieve weather.");
            }

            return Ok(new WeatherDto
            {
                Date = DateTime.UtcNow,
                Temperature = weather.CurrentWeather?.Temperature ?? 0,
                Condition = WeatherCodeMapper.ToCondition(weather.CurrentWeather?.Weathercode ?? 0),
                Humidity = 0,
                WindSpeed = weather.CurrentWeather?.Windspeed ?? 0
            });
        }
        // ✅ FORECAST
        [HttpGet("forecast/{destinationId}")]
        public async Task<IActionResult> GetForecast(int destinationId)
        {
            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == destinationId);

            if (destination == null)
                return NotFound("Destination not found.");

            var forecast = await _weatherService.GetForecast(
                destination.Latitude,
                destination.Longitude);

            if (forecast?.Daily == null)
                return StatusCode(500, "Failed to retrieve forecast.");

            var result = forecast.Daily.Time!
                .Select((date, index) => new WeatherDto
                {
                    Date = DateTime.Parse(date),
                    Temperature =
                        ((forecast.Daily.TemperatureMax?[index] ?? 0) +
                         (forecast.Daily.TemperatureMin?[index] ?? 0)) / 2,
                    Condition = WeatherCodeMapper.ToCondition(
    forecast.Daily.WeatherCode?[index] ?? 0),
                    Humidity = 0,
                    WindSpeed = 0
                })
                .ToList();

            return Ok(result);
        }
    }
}
