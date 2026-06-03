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

        public TripController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL TRIPS (ONLY USER'S)
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trips = await _context.Trips
                .Where(t => t.UserId == userId)
                .Include(t => t.Destination)
                .ToListAsync();

            return Ok(trips);
        }

        // ✅ GET SINGLE TRIP
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrip(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trip = await _context.Trips
                .Include(t => t.Days)
                .ThenInclude(d => d.Activities)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
                return NotFound();

            return Ok(trip);
        }

        // ✅ CREATE TRIP
        [HttpPost]
        public async Task<IActionResult> CreateTrip(Trip trip)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            trip.UserId = userId; // 🔥 IMPORTANT

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return Ok(trip);
        }

        // ✅ UPDATE TRIP
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrip(int id, Trip updatedTrip)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
                return NotFound();

            trip.Title = updatedTrip.Title;
            trip.StartDate = updatedTrip.StartDate;
            trip.EndDate = updatedTrip.EndDate;
            trip.Status = updatedTrip.Status;

            await _context.SaveChangesAsync();

            return Ok(trip);
        }

        // ✅ DELETE TRIP
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (trip == null)
                return NotFound();

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            return Ok("Trip deleted");
        }
    }
}
