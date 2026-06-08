using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/destinations")]
    [Authorize]
    public class DestinationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<DestinationController> _logger;

        public DestinationController(AppDbContext context, IMemoryCache cache, ILogger<DestinationController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid user claim.");

            return userId;
        }
        // ✅ GET ALL (PUBLIC)
        [AllowAnonymous]
        [HttpGet] // SAME endpoint
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation(
    "Fetching destinations. Search: {Search}, Page: {Page}, PageSize: {PageSize}",
    search,
    page,
    pageSize);

            var query = _context.Destinations.AsQueryable();

            // 🔍 SEARCH
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.Name.ToLower().Contains(search.ToLower()) ||
                    d.Country.ToLower().Contains(search.ToLower()));
            }
            // 📊 TOTAL COUNT
            var totalCount = await query.CountAsync();

            // 📄 PAGINATION
            var data = await query
                .OrderBy(d => d.Name) // optional but recommended
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation(
    "Retrieved {Count} destinations out of {TotalCount} total results",
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

        // ✅ GET BY ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation(
       "Fetching destination. DestinationId: {DestinationId}",
       id);

            var dest = await _context.Destinations.FindAsync(id);
            if (dest == null)
            {
                _logger.LogWarning(
                    "Destination not found. DestinationId: {DestinationId}",
                    id);

                return NotFound();
            }
            _logger.LogInformation(
       "Destination retrieved successfully. DestinationId: {DestinationId}",
       id);
            return Ok(dest);
        }

        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Destination dest)
        {
            var userId = GetUserId();

            _logger.LogInformation(
    "Admin {UserId} creating destination {DestinationName}",
    userId,
    dest.Name);

            _context.Destinations.Add(dest);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
    "Admin {UserId} created destination {DestinationId}",
    userId,
    dest.Id);

            return Ok(dest);
        }

        // ✅ UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Destination updated)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Admin {UserId} updating destination {DestinationId}",
                userId,
                id);

            var dest = await _context.Destinations.FindAsync(id);
            if (dest == null)
            {
                _logger.LogWarning(
     "Admin {UserId} attempted to update non-existent destination {DestinationId}",
     userId,
     id);

                return NotFound();
            }

            dest.Name = updated.Name;
            dest.Country = updated.Country;
            dest.Description = updated.Description;
            dest.ImageUrl = updated.ImageUrl;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
    "Admin {UserId} updated destination {DestinationId}",
    userId,
    id);

            _cache.Remove($"destination_overview_{id}");

            _logger.LogInformation(
    "Cache invalidated for destination overview. DestinationId: {DestinationId}",
    id);

            return Ok(dest);
        }

        // ✅ DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Admin {UserId} deleting destination {DestinationId}",
                userId,
                id);

            var dest = await _context.Destinations.FindAsync(id);
            if (dest == null)
            {
                _logger.LogWarning(
     "Admin {UserId} attempted to delete non-existent destination {DestinationId}",
     userId,
     id);

                return NotFound();
            }

            _context.Destinations.Remove(dest);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
    "Admin {UserId} deleted destination {DestinationId}",
    userId,
    id);

            _cache.Remove($"destination_overview_{id}");

            _logger.LogInformation(
    "Cache invalidated for deleted destination. DestinationId: {DestinationId}",
    id);

            return Ok("Deleted");
        }
    }
}
