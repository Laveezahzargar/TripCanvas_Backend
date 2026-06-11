using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.Services;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/places")]
    public class PlacesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PlacesController> _logger;
        private readonly DestinationService _destinationService;

        public PlacesController(AppDbContext context, ILogger<PlacesController> logger, DestinationService destinationService)
        {
            _context = context;
            _logger = logger;
            _destinationService = destinationService;
        }

        // ✅ GET BY DESTINATION
        [HttpGet("{destinationId}")]
        public async Task<IActionResult> GetByDestination(int destinationId)
        {
            _logger.LogInformation(
        "Fetching places for DestinationId: {DestinationId}",
        destinationId);

            var destination = await _context.Destinations
            .FirstOrDefaultAsync(d => d.Id == destinationId);

            if (destination == null)
                return NotFound("Destination not found.");

            _logger.LogInformation(
    "Destination {Id}: Lat={Lat}, Lon={Lon}",
    destination.Id,
    destination.Latitude,
    destination.Longitude);

            var response = await _destinationService.GetPlaces(
           destination.Latitude,
           destination.Longitude);

            if (response == null)
                return StatusCode(500);

            var places = response.Features?
    .Select((f, index) => new
    {
        Id = index + 1,
        Name = f.Properties?.Name,
        Address = f.Properties?.Address,
        Latitude = f.Geometry?.Coordinates?[1],
        Longitude = f.Geometry?.Coordinates?[0]
    })
    .ToList();

            _logger.LogInformation(
    "Retrieved places successfully for DestinationId: {DestinationId}",
    destinationId);

            return Ok(places);
        }

    //    // ✅ FILTER
    //    [HttpGet] // SAME endpoint, just upgraded
    //    public async Task<IActionResult> GetPlaces(
    //        [FromQuery] int destinationId,
    //        [FromQuery] string? category,
    //        [FromQuery] string? search,
    //        [FromQuery] int page = 1,
    //        [FromQuery] int pageSize = 10)
    //    {
    //        _logger.LogInformation(
    //"Fetching places. DestinationId: {DestinationId}, Category: {Category}, Search: {Search}, Page: {Page}, PageSize: {PageSize}",
    //destinationId,
    //category,
    //search,
    //page,
    //pageSize);

    //        var query = _context.Places.AsQueryable();

    //        if (destinationId != 0)
    //            query = query.Where(p => p.DestinationId == destinationId);

    //        if (!string.IsNullOrEmpty(category))
    //            query = query.Where(p => p.Category.ToLower() == category.ToLower());

    //        if (!string.IsNullOrEmpty(search))
    //            query = query.Where(p =>
    //                p.Name.ToLower().Contains(search.ToLower()) ||
    //                p.Location.ToLower().Contains(search.ToLower()));

    //        var totalCount = await query.CountAsync();

    //        var data = await query
    //            .Skip((page - 1) * pageSize)
    //            .Take(pageSize)
    //            .ToListAsync();

    //        if (totalCount == 0)
    //        {
    //            _logger.LogWarning(
    //                "No places found. DestinationId: {DestinationId}, Category: {Category}, Search: {Search}",
    //                destinationId,
    //                category,
    //                search);
    //        }

    //        _logger.LogInformation(
    //"Retrieved {Count} places out of {TotalCount} total results",
    //data.Count,
    //totalCount);

    //        return Ok(new
    //        {
    //            totalCount,
    //            page,
    //            pageSize,
    //            data
    //        });
    //    }
    }
}
