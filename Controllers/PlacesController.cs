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

        public PlacesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET BY DESTINATION
        [HttpGet("{destinationId}")]
        public async Task<IActionResult> GetByDestination(int destinationId)
        {
            var places = await _context.Places
                .Where(p => p.DestinationId == destinationId)
                .ToListAsync();

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
