using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.DTOs;
using P6_Travel_Planner_Backend.Models;
using System.Security.Claims;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class ActivityController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ActivityController> _logger;


        public ActivityController(AppDbContext context, ILogger<ActivityController> logger)
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

        // ✅ GET ACTIVITIES FOR A DAY
        [HttpGet("days/{dayId}/activities")]
        public async Task<IActionResult> GetActivities(int dayId)
        {
            var userId = GetUserId();

            _logger.LogInformation(
    "Fetching activities for DayId: {DayId}, UserId: {UserId}",
    dayId,
    userId);

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == dayId && d.Trip.UserId == userId);

            if (day == null)
            {
                _logger.LogWarning(
                    "Day not found while fetching activities. DayId: {DayId}, UserId: {UserId}",
                    dayId,
                    userId);

                return NotFound("Day not found or unauthorized");
            }

            var activities = await _context.Activities
     .Where(a => a.ItineraryDayId == dayId)
     .OrderBy(a => a.StartTime)
     .Select(a => new ActivityDto
     {
         Id = a.Id,
         ItineraryDayId = a.ItineraryDayId,
         Name = a.Name,
         Location = a.Location,
         Cost = a.Cost,
         StartTime = a.StartTime,
         EndTime = a.EndTime
     })
     .ToListAsync();

            _logger.LogInformation(
    "Retrieved {ActivityCount} activities for DayId: {DayId}",
    activities.Count,
    dayId);

            return Ok(activities);
        }

        // ✅ ADD ACTIVITY
        [HttpPost("days/{dayId}/activities")]
        public async Task<IActionResult> CreateActivity(int dayId, Activity activity)
        {
            var userId = GetUserId();

            _logger.LogInformation(
    "Creating activity '{ActivityName}' for DayId: {DayId}, UserId: {UserId}",
    activity.Name,
    dayId,
    userId);

            var day = await _context.ItineraryDays
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == dayId && d.Trip.UserId == userId);

            if (day == null)
            {
                _logger.LogWarning(
    "Create activity failed. Day not found. DayId: {DayId}, UserId: {UserId}",
    dayId,
    userId);
                return NotFound("Day not found or unauthorized");
            }

            activity.ItineraryDayId = dayId;

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
    "Activity created successfully. ActivityId: {ActivityId}, DayId: {DayId}",
    activity.Id,
    dayId);

            return Ok(activity);
        }

        // ✅ UPDATE ACTIVITY
        [HttpPut("activities/{id}")]
        public async Task<IActionResult> UpdateActivity(int id, Activity updatedActivity)
        {
            var userId = GetUserId();

            _logger.LogInformation(
    "Updating ActivityId: {ActivityId}, UserId: {UserId}",
    id,
    userId);

            var activity = await _context.Activities
                .Include(a => a.ItineraryDay)
                .ThenInclude(d => d.Trip)
                .FirstOrDefaultAsync(a => a.Id == id && a.ItineraryDay.Trip.UserId == userId);

            if (activity == null)
            {
                _logger.LogWarning(
    "Update failed. Activity not found. ActivityId: {ActivityId}, UserId: {UserId}",
    id,
    userId);
                return NotFound();
            }

            activity.Name = updatedActivity.Name;
            activity.Location = updatedActivity.Location;
            activity.Cost = updatedActivity.Cost;
            activity.StartTime = updatedActivity.StartTime;
            activity.EndTime = updatedActivity.EndTime;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
    "Activity updated successfully. ActivityId: {ActivityId}, UserId: {UserId}",
    id,
    userId);

            return Ok(activity);
        }

        // ✅ DELETE ACTIVITY
        [HttpDelete("activities/{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var userId = GetUserId();

            _logger.LogInformation(
    "Deleting ActivityId: {ActivityId}, UserId: {UserId}",
    id,
    userId);

            var activity = await _context.Activities
                .Include(a => a.ItineraryDay)
                .ThenInclude(d => d.Trip)
                .FirstOrDefaultAsync(a => a.Id == id && a.ItineraryDay.Trip.UserId == userId);

            if (activity == null)
            {
                _logger.LogWarning(
    "Delete failed. Activity not found. ActivityId: {ActivityId}, UserId: {UserId}",
    id,
    userId);
                return NotFound();
            }
                

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
    "Activity deleted successfully. ActivityId: {ActivityId}, UserId: {UserId}",
    id,
    userId);


            return Ok("Activity deleted");
        }
    }
}
