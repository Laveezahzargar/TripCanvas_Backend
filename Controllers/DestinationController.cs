using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/destinations")]
    [Authorize]
    public class DestinationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public DestinationController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // ✅ GET ALL (PUBLIC)
        [AllowAnonymous]
        [HttpGet] // SAME endpoint
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
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
            var dest = await _context.Destinations.FindAsync(id);
            if (dest == null) return NotFound();
            return Ok(dest);
        }

        // 🔐 ADMIN ONLY
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Destination dest)
        {
            _context.Destinations.Add(dest);
            await _context.SaveChangesAsync();

            return Ok(dest);
        }

        // ✅ UPDATE
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Destination updated)
        {
            var dest = await _context.Destinations.FindAsync(id);
            if (dest == null) return NotFound();

            dest.Name = updated.Name;
            dest.Country = updated.Country;
            dest.Description = updated.Description;
            dest.ImageUrl = updated.ImageUrl;

            await _context.SaveChangesAsync();

            _cache.Remove($"destination_overview_{id}");

            return Ok(dest);
        }

        // ✅ DELETE
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dest = await _context.Destinations.FindAsync(id);
            if (dest == null) return NotFound();

            _context.Destinations.Remove(dest);
            await _context.SaveChangesAsync();

            _cache.Remove($"destination_overview_{id}");

            return Ok("Deleted");
        }
    }
}
