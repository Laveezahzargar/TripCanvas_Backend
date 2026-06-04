using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using P6_Travel_Planner_Backend.Data;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/destinations")]
    public class DestinationOverviewController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DestinationOverviewController> _logger;

        public DestinationOverviewController(AppDbContext context, IMemoryCache cache, ILogger<DestinationOverviewController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // 🌍 OVERVIEW API
        [HttpGet("{id}/overview")]
        public async Task<IActionResult> GetOverview(int id)
        {
            string cacheKey = $"destination_overview_{id}";

            _logger.LogInformation(
               "Fetching destination overview. DestinationId: {DestinationId}",
               id);

            // ✅ TRY GET FROM CACHE
            if (_cache.TryGetValue(cacheKey, out object? cachedData))
            {
                _logger.LogInformation(
                    "Cache hit for destination overview. DestinationId: {DestinationId}",
                    id);
                return Ok(cachedData); // ⚡ super fast
            }


            _logger.LogInformation(
                "Cache miss for destination overview. DestinationId: {DestinationId}",
                id);

            // ❌ Not in cache → fetch from DB
            var destination = await _context.Destinations
                .FirstOrDefaultAsync(d => d.Id == id);

            if (destination == null)
            {
                _logger.LogWarning(
                    "Destination not found. DestinationId: {DestinationId}",
                    id);

                return NotFound("Destination not found");
            }

            var weather = await _context.Weather
                .Where(w => w.DestinationId == id)
                .OrderByDescending(w => w.Date)
                .FirstOrDefaultAsync();

            var allPlaces = await _context.Places
                .Where(p => p.DestinationId == id)
                .ToListAsync();

            var result = new
            {
                destination,
                weather,
                places = new
                {
                    hotels = allPlaces.Where(p => p.Category == "Hotel").Take(3),
                    restaurants = allPlaces.Where(p => p.Category == "Restaurant").Take(3),
                    attractions = allPlaces.Where(p => p.Category == "Attraction").Take(3)
                },
                topPlaces = allPlaces.OrderByDescending(p => p.Rating).Take(5)
            };

            // ✅ STORE IN CACHE (IMPORTANT)
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)); // ⏱️ cache time

            _cache.Set(cacheKey, result, cacheOptions);

            _logger.LogInformation(
    "Destination overview generated and cached successfully. DestinationId: {DestinationId}",
    id, allPlaces.Count());

            return Ok(result);
        }
    }
}
