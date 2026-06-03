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
    public class ActivityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActivityController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ACTIVITIES FOR A DAY
        [HttpGet("days/{dayId}/activities")]
        public async Task<IActionResult> GetActivities(int dayId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == dayId && d.Trip.UserId == userId);

            if (day == null)
                return NotFound("Day not found or unauthorized");

            var activities = await _context.Activities
                .Where(a => a.ItineraryDayId == dayId)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            return Ok(activities);
        }

        // ✅ ADD ACTIVITY
        [HttpPost("days/{dayId}/activities")]
        public async Task<IActionResult> CreateActivity(int dayId, Activity activity)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == dayId && d.Trip.UserId == userId);

            if (day == null)
                return NotFound("Day not found or unauthorized");

            activity.ItineraryDayId = dayId;

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return Ok(activity);
        }

        // ✅ UPDATE ACTIVITY
        [HttpPut("activities/{id}")]
        public async Task<IActionResult> UpdateActivity(int id, Activity updatedActivity)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var activity = await _context.Activities
                .Include(a => a.ItineraryDay)
                .ThenInclude(d => d.Trip)
                .FirstOrDefaultAsync(a => a.Id == id && a.ItineraryDay.Trip.UserId == userId);

            if (activity == null)
                return NotFound();

            activity.Name = updatedActivity.Name;
            activity.Location = updatedActivity.Location;
            activity.Cost = updatedActivity.Cost;
            activity.StartTime = updatedActivity.StartTime;
            activity.EndTime = updatedActivity.EndTime;

            await _context.SaveChangesAsync();

            return Ok(activity);
        }

        // ✅ DELETE ACTIVITY
        [HttpDelete("activities/{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var activity = await _context.Activities
                .Include(a => a.ItineraryDay)
                .ThenInclude(d => d.Trip)
                .FirstOrDefaultAsync(a => a.Id == id && a.ItineraryDay.Trip.UserId == userId);

            if (activity == null)
                return NotFound();

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();


            return Ok("Activity deleted");
        }
    }
}
