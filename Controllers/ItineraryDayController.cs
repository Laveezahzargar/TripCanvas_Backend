using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.DTOs;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ItineraryDayController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ItineraryDayController> _logger;

        public ItineraryDayController(AppDbContext context, ILogger<ItineraryDayController> logger)
        {
            _context = context;
            _logger = logger;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("Invalid user claim.");

            return userId;
        }

        // ✅ GET ALL DAYS FOR A TRIP
        [HttpGet("trips/{tripId}/days")]
        public async Task<IActionResult> GetDays(int tripId)
        {
            var userId = GetUserId();

            _logger.LogInformation(
       "Fetching itinerary days for TripId: {TripId}, UserId: {UserId}",
       tripId,
       userId);

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
            {
                _logger.LogWarning(
                    "Trip not found while fetching days. TripId: {TripId}, UserId: {UserId}",
                    tripId,
                    userId);

                return NotFound("Trip not found or unauthorized");
            }

            var days = await _context.ItineraryDays
       .Where(d => d.TripId == tripId)
       .OrderBy(d => d.DayNumber)
       .Select(d => new ItineraryDayDto
       {
           Id = d.Id,
           TripId = d.TripId,
           Date = d.Date,
           DayNumber = d.DayNumber,
           Notes = d.Notes
       })
       .ToListAsync();

            _logger.LogInformation(
        "Retrieved {DayCount} itinerary days for TripId: {TripId}",
        days.Count,
        tripId);

            return Ok(days);
        }

        // ✅ CREATE DAY
        [HttpPost("trips/{tripId}/days")]
        public async Task<IActionResult> CreateDay(int tripId, ItineraryDay day)
        {
            var userId = GetUserId();

            _logger.LogInformation(
       "Creating itinerary day for TripId: {TripId}, UserId: {UserId}",
       tripId,
       userId);

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.UserId == userId);

            if (trip == null)
            {
                _logger.LogWarning(
                    "Create day failed. Trip not found. TripId: {TripId}, UserId: {UserId}",
                    tripId,
                    userId);

                return NotFound("Trip not found or unauthorized");
            }

            day.TripId = tripId;

            _context.ItineraryDays.Add(day);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
       "Itinerary day created successfully. DayId: {DayId}, TripId: {TripId}",
       day.Id,
       tripId);

            return Ok(day);
        }

        // ✅ GET SINGLE DAY
        [HttpGet("days/{id}")]
        public async Task<IActionResult> GetDay(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
       "Fetching itinerary day. DayId: {DayId}, UserId: {UserId}",
       id,
       userId);

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

            if (day == null)
            {
                _logger.LogWarning(
                    "Itinerary day not found. DayId: {DayId}, UserId: {UserId}",
                    id,
                    userId);

                return NotFound();
            }

            _logger.LogInformation(
    "Itinerary day retrieved successfully. DayId: {DayId}, UserId: {UserId}",
    id,
    userId);

            return Ok(day);
        }

        // ✅ UPDATE DAY
        [HttpPut("days/{id}")]
        public async Task<IActionResult> UpdateDay(int id, ItineraryDay updatedDay)
        {
            var userId = GetUserId();

            _logger.LogInformation(
       "Updating itinerary day. DayId: {DayId}, UserId: {UserId}",
       id,
       userId);

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

            if (day == null)
            {
                _logger.LogWarning(
                    "Update failed. Day not found. DayId: {DayId}, UserId: {UserId}",
                    id,
                    userId);

                return NotFound();
            }

            day.Date = updatedDay.Date;
            day.DayNumber = updatedDay.DayNumber;
            day.Notes = updatedDay.Notes;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
        "Itinerary day updated successfully. DayId: {DayId}",
        id);

            return Ok(day);
        }

        // ✅ DELETE DAY
        [HttpDelete("days/{id}")]
        public async Task<IActionResult> DeleteDay(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
       "Deleting itinerary day. DayId: {DayId}, UserId: {UserId}",
       id,
       userId);

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == id && d.Trip.UserId == userId);

            if (day == null)
            {
                _logger.LogWarning(
                    "Delete failed. Day not found. DayId: {DayId}, UserId: {UserId}",
                    id,
                    userId);

                return NotFound();
            }

            _context.ItineraryDays.Remove(day);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
       "Itinerary day deleted successfully. DayId: {DayId}",
       id);

            return Ok("Day deleted");
        }
    }
}
