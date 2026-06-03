using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ItineraryDayController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItineraryDayController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL DAYS FOR A TRIP
        [HttpGet("trips/{tripId}/days")]
        public async Task<IActionResult> GetDays(int tripId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return NotFound("Trip not found or unauthorized");

            var days = await _context.ItineraryDays
                .Where(d => d.TripId == tripId)
                .OrderBy(d => d.DayNumber)
                .ToListAsync();

            return Ok(days);
        }

        // ✅ CREATE DAY
        [HttpPost("trips/{tripId}/days")]
        public async Task<IActionResult> CreateDay(int tripId, ItineraryDay day)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
                return NotFound("Trip not found or unauthorized");

            day.TripId = tripId;

            _context.ItineraryDays.Add(day);
            await _context.SaveChangesAsync();

            return Ok(day);
        }

        // ✅ GET SINGLE DAY
        [HttpGet("days/{id}")]
        public async Task<IActionResult> GetDay(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

            if (day == null)
                return NotFound();

            return Ok(day);
        }

        // ✅ UPDATE DAY
        [HttpPut("days/{id}")]
        public async Task<IActionResult> UpdateDay(int id, ItineraryDay updatedDay)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

            if (day == null)
                return NotFound();

            day.Date = updatedDay.Date;
            day.DayNumber = updatedDay.DayNumber;
            day.Notes = updatedDay.Notes;

            await _context.SaveChangesAsync();

            return Ok(day);
        }

        // ✅ DELETE DAY
        [HttpDelete("days/{id}")]
        public async Task<IActionResult> DeleteDay(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

            if (day == null)
                return NotFound();

            _context.ItineraryDays.Remove(day);
            await _context.SaveChangesAsync();

            return Ok("Day deleted");
        }
    }
}
