using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.Models;
using System.Security.Claims;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/trips")]
    [Authorize] // 🔐 ALL endpoints protected
    public class TripController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TripController> _logger;

        public TripController(AppDbContext context, ILogger<TripController> logger)
        {
            _context = context;
            _logger = logger;
        }
        private int GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user claim.");
            }

            return userId;
        }
        // ✅ GET ALL TRIPS (ONLY USER'S)
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var userId = GetUserId();

            _logger.LogInformation("Fetching trips for UserId: {UserId}", userId);

             var trips = await _context.Trips
            .Where(t => t.UserId == userId)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.DestinationId,
                Destination = t.Destination.Name,
                t.StartDate,
                t.EndDate,
                Status = (int)t.Status
            })
            .ToListAsync();

            _logger.LogInformation(
                "Retrieved {TripCount} trips for UserId: {UserId}",
                trips.Count,
                userId);

            return Ok(trips);
        }

        // ✅ GET SINGLE TRIP
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Fetching TripId: {TripId} for UserId: {UserId}",
                id,
                userId);


            var trip = await _context.Trips
                .Include(t => t.Days)
                .ThenInclude(d => d.Activities)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
            {
                _logger.LogWarning(
                    "Trip not found. TripId: {TripId}, UserId: {UserId}",
                    id,
                    userId);
                return NotFound();
            }

            _logger.LogInformation(
    "Trip retrieved successfully. TripId: {TripId}, UserId: {UserId}",
    id,
    userId);

            return Ok(trip);
        }

        // ✅ CREATE TRIP
        [HttpPost]
        public async Task<IActionResult> CreateTrip(Trip trip)
        {
            if (!ModelState.IsValid)
            {
                foreach (var item in ModelState)
                {
                    foreach (var error in item.Value.Errors)
                    {
                        _logger.LogError(
                            "Field: {Field}, Error: {Error}",
                            item.Key,
                            error.ErrorMessage);
                    }
                }

                return BadRequest(ModelState);
            }
            var userId = GetUserId();

            _logger.LogInformation(
               "Creating trip '{Title}' for UserId: {UserId}",
               trip.Title,
               userId);

            trip.UserId = userId; // 🔥 IMPORTANT

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
        "Trip created successfully. TripId: {TripId}, UserId: {UserId}",
        trip.Id,
        userId);

            return Ok(trip);
        }

        // ✅ UPDATE TRIP
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(int id, Trip updatedTrip)
        {
            var userId = GetUserId();

            _logger.LogInformation(
       "Updating TripId: {TripId} for UserId: {UserId}",
       id,
       userId);

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
            {
                _logger.LogWarning(
                    "Update failed. Trip not found. TripId: {TripId}, UserId: {UserId}",
                    id,
                    userId);

                return NotFound();
            }

            trip.Title = updatedTrip.Title;
            trip.StartDate = updatedTrip.StartDate;
            trip.EndDate = updatedTrip.EndDate;
            trip.Status = updatedTrip.Status;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
       "Trip updated successfully. TripId: {TripId}, UserId: {UserId}",
       id,
       userId);

            return Ok(trip);
        }

        // ✅ DELETE TRIP
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
        "Deleting TripId: {TripId} for UserId: {UserId}",
        id,
        userId);

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
            {
                _logger.LogWarning(
                    "Delete failed. Trip not found. TripId: {TripId}, UserId: {UserId}",
                    id,
                    userId);

                return NotFound();
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
        "Trip deleted successfully. TripId: {TripId}, UserId: {UserId}",
        id,
        userId);

            return Ok("Trip deleted");
        }
    }
}
