using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.DTOs;
using P6_Travel_Planner_Backend.Enums;
using P6_Travel_Planner_Backend.Models;
using System.Linq.Expressions;
using System.Security.Claims;
using P6_Travel_Planner_Backend.Enums;

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
            try
            {
                var userId = GetUserId();

                _logger.LogInformation(
                    "Fetching TripId: {TripId} for UserId: {UserId}",
                    id,
                    userId);


                var trip = await _context.Trips
                 .Where(t => t.Id == id && t.UserId == userId)
                 .Select(t => new
                 {
                     t.Id,
                     t.Title,
                     t.DestinationId,
                     Destination = t.Destination != null ? t.Destination.Name : "",
                     t.StartDate,
                     t.EndDate,
                     Status = (int)t.Status
                 })
                 .FirstOrDefaultAsync();

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
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access while fetching TripId: {TripId}", id);
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching TripId: {TripId}", id);
                return StatusCode(500, "An error occurred while fetching the trip.");
            }
        }
        
        

        // ✅ CREATE TRIP
        [HttpPost]
        public async Task<IActionResult> CreateTrip(CreateTripDto dto)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Creating trip '{Title}' for UserId: {UserId}",
                dto.Title,
                userId);

            if (dto.EndDate < dto.StartDate)
                return BadRequest("EndDate cannot be before StartDate");

            var trip = new Trip
            {
                Title = dto.Title,
                DestinationId = dto.DestinationId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = TripStatus.InProgress,
                UserId = userId
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Trip created successfully. TripId: {TripId}, UserId: {UserId}",
                trip.Id,
                userId);

            return CreatedAtAction(
                nameof(GetTrip),
                new { id = trip.Id },
                new
                {
                    trip.Id,
                    trip.Title,
                    trip.DestinationId,
                    trip.StartDate,
                    trip.EndDate,
                    Status = (int)trip.Status
                });
        }

        // ✅ UPDATE TRIP
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(int id, UpdateTripDto dto)
        {
            var userId = GetUserId();

            _logger.LogInformation(
                "Updating TripId: {TripId} for UserId: {UserId}",
                id,
                userId);

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
                return NotFound();

            if (dto.EndDate < dto.StartDate)
                return BadRequest("EndDate cannot be before StartDate");

            trip.Title = dto.Title;
            trip.StartDate = dto.StartDate;
            trip.EndDate = dto.EndDate;
            trip.Status = dto.Status;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Trip updated successfully. TripId: {TripId}, UserId: {UserId}",
                id,
                userId);

            return Ok(new
            {
                trip.Id,
                trip.Title,
                trip.DestinationId,
                trip.StartDate,
                trip.EndDate,
                Status = (int)trip.Status
            });
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
