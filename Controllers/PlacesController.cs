using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/places")]
    public class PlacesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PlacesController> _logger;

        public PlacesController(AppDbContext context, ILogger<PlacesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ GET BY DESTINATION
        [HttpGet("{destinationId}")]
        public async Task<IActionResult> GetByDestination(int destinationId)
        {
            _logger.LogInformation(
        "Fetching places for DestinationId: {DestinationId}",
        destinationId);

            var places = await _context.Places
                .Where(p => p.DestinationId == destinationId)
                .ToListAsync();

            _logger.LogInformation(
       "Retrieved {PlaceCount} places for DestinationId: {DestinationId}",
       places.Count,
       destinationId);


            return Ok(places);
        }

        // ✅ FILTER
        [HttpGet] // SAME endpoint, just upgraded
        public async Task<IActionResult> GetPlaces(
            [FromQuery] int destinationId,
            [FromQuery] string? category,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation(
    "Fetching places. DestinationId: {DestinationId}, Category: {Category}, Search: {Search}, Page: {Page}, PageSize: {PageSize}",
    destinationId,
    category,
    search,
    page,
    pageSize);

            var query = _context.Places.AsQueryable();

            if (destinationId != 0)
                query = query.Where(p => p.DestinationId == destinationId);

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.ToLower() == category.ToLower());

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search.ToLower()) ||
                    p.Location.ToLower().Contains(search.ToLower()));

            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (totalCount == 0)
            {
                _logger.LogWarning(
                    "No places found. DestinationId: {DestinationId}, Category: {Category}, Search: {Search}",
                    destinationId,
                    category,
                    search);
            }

            _logger.LogInformation(
    "Retrieved {Count} places out of {TotalCount} total results",
    data.Count,
    totalCount);

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                data
            });
        }
    }
}
